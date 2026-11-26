using System.Text.Json;
using AntiScam.Application.Common;
using AntiScam.Application.DTOs.Alert;
using AntiScam.Application.DTOs.TravelRoute;
using AntiScam.Domain.Entities;
using AntiScam.Domain.Enums;
using AntiScam.Domain.Interfaces;
using AntiScam.Domain.ValueObjects;
using AntiScam.Domain.Interfaces;
using AntiScam.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AntiScam.Application.Services;

public interface ITravelRouteService
{
    Task<Result<TravelRouteDto>> CreateRouteAsync(CreateTravelRouteRequest request, string userId);
    Task<Result> StartTrackingAsync(Guid routeId, string userId);
    Task<Result> StopTrackingAsync(Guid routeId, string userId);
    Task<Result<LocationReportResult>> ReportLocationAsync(ReportLocationRequest request, string userId);
    Task<Result> RespondToAlertAsync(AlertResponseRequest request, string userId);
    Task<Result<TravelRouteDto?>> GetActiveRouteAsync(string userId);
    Task<Result<List<TravelRouteDto>>> GetUserRoutesAsync(string userId);
    Task<Result> TriggerSosAsync(Guid routeId, string userId);
}

public class TravelRouteService : ITravelRouteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRouteDeviationDetector _deviationDetector;
    private readonly IAlertCountdownService _countdownService;
    private readonly IEmailService _emailService;
    private readonly ITelegramService _telegramService;

    // Track consecutive deviations in-memory for real-time safety checks
    private static readonly Dictionary<Guid, int> _consecutiveDeviations = new();

    public TravelRouteService(
        IUnitOfWork unitOfWork,
        IRouteDeviationDetector deviationDetector,
        IAlertCountdownService countdownService,
        IEmailService emailService,
        ITelegramService telegramService)
    {
        _unitOfWork = unitOfWork;
        _deviationDetector = deviationDetector;
        _countdownService = countdownService;
        _emailService = emailService;
        _telegramService = telegramService;
    }

    public async Task<Result<TravelRouteDto>> CreateRouteAsync(CreateTravelRouteRequest request, string userId)
    {
        var route = new TravelRoute
        {
            UserId = userId,
            Name = request.RouteName,
            WaypointsJson = JsonSerializer.Serialize(request.Waypoints),
            DeviationThresholdMeters = request.DeviationThresholdMeters,
            Status = RouteStatus.Planned,
            IsActive = true,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.TravelRoutes.AddAsync(route);
        await _unitOfWork.SaveChangesAsync();

        return Result<TravelRouteDto>.Success(MapToDto(route));
    }

    public async Task<Result> StartTrackingAsync(Guid routeId, string userId)
    {
        var route = await _unitOfWork.TravelRoutes.Query()
            .FirstOrDefaultAsync(r => r.Id == routeId && r.UserId == userId);

        if (route == null) return Result.Failure("Không tìm thấy lộ trình.");

        route.Status = RouteStatus.Active;
        route.StartedAt = DateTime.UtcNow;
        _consecutiveDeviations[routeId] = 0;

        await _unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> StopTrackingAsync(Guid routeId, string userId)
    {
        var route = await _unitOfWork.TravelRoutes.Query()
            .FirstOrDefaultAsync(r => r.Id == routeId && r.UserId == userId);

        if (route == null) return Result.Failure("Không tìm thấy lộ trình.");

        route.Status = RouteStatus.Completed;
        route.EndedAt = DateTime.UtcNow;
        _consecutiveDeviations.Remove(routeId);

        await _unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<LocationReportResult>> ReportLocationAsync(ReportLocationRequest request, string userId)
    {
        var route = await _unitOfWork.TravelRoutes.Query()
            .FirstOrDefaultAsync(r => r.Id == request.TravelRouteId && r.UserId == userId);

        if (route == null || route.Status != RouteStatus.Active)
        {
            return Result<LocationReportResult>.Failure("Lộ trình không tồn tại hoặc chưa kích hoạt theo dõi.");
        }

        var currentPoint = new GeoPoint(request.Latitude, request.Longitude);
        double distance = _deviationDetector.GetDistanceToRouteMeters(currentPoint, route);
        bool isDeviating = distance > route.DeviationThresholdMeters;

        if (!_consecutiveDeviations.TryGetValue(route.Id, out int consecutive))
        {
            consecutive = 0;
        }

        if (isDeviating) consecutive++;
        else consecutive = 0;

        _consecutiveDeviations[route.Id] = consecutive;

        // Save location log
        var log = new LocationLog
        {
            TravelRouteId = route.Id,
            Latitude = (decimal)request.Latitude,
            Longitude = (decimal)request.Longitude,
            Accuracy = (decimal?)request.Accuracy,
            Speed = (decimal?)request.Speed,
            Bearing = (decimal?)request.Bearing,
            RecordedAt = DateTime.UtcNow,
            DistanceFromRoute = distance,
            IsDeviation = isDeviating
        };

        await _unitOfWork.LocationLogs.AddAsync(log);

        Guid? alertId = null;
        string message = "Vị trí trong vùng an toàn.";

        // Anti-jitter: Only trigger alert after 3 consecutive deviations
        if (consecutive >= 3)
        {
            message = "CẢNH BÁO: Bạn đã đi lệch khỏi lộ trình! Vui lòng xác nhận an toàn.";

            var alert = new RouteAlert
            {
                TravelRouteId = route.Id,
                AlertType = AlertType.Deviation,
                Resolution = AlertResolution.Expired,
                Message = $"Lệch lộ trình {distance:F0}m (vượt ngưỡng {route.DeviationThresholdMeters}m)",
                Latitude = (decimal)request.Latitude,
                Longitude = (decimal)request.Longitude,
                CountdownSeconds = 300,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.RouteAlerts.AddAsync(alert);
            await _unitOfWork.SaveChangesAsync();

            alertId = alert.Id;
            _countdownService.StartCountdown(alert.Id, 300);
        }
        else
        {
            await _unitOfWork.SaveChangesAsync();
        }

        return Result<LocationReportResult>.Success(new LocationReportResult
        {
            IsDeviating = consecutive >= 3,
            CurrentDistanceMeters = Math.Round(distance, 1),
            AlertId = alertId,
            ConsecutiveDeviationCount = consecutive,
            Message = message
        });
    }

    public async Task<Result> RespondToAlertAsync(AlertResponseRequest request, string userId)
    {
        var alert = await _unitOfWork.RouteAlerts.Query()
            .Include(a => a.TravelRoute)
            .FirstOrDefaultAsync(a => a.Id == request.AlertId && a.TravelRoute.UserId == userId);

        if (alert == null) return Result.Failure("Không tìm thấy cảnh báo.");

        _countdownService.CancelCountdown(alert.Id);

        if (request.Action == AlertActionResponse.ImOkay)
        {
            alert.Resolution = AlertResolution.UserConfirmedSafe;
            alert.ResolvedAt = DateTime.UtcNow;
            _consecutiveDeviations[alert.TravelRouteId] = 0;
        }
        else if (request.Action == AlertActionResponse.ChangeRoute && request.NewWaypoints != null)
        {
            alert.Resolution = AlertResolution.UserChangedRoute;
            alert.ResolvedAt = DateTime.UtcNow;
            alert.TravelRoute.WaypointsJson = JsonSerializer.Serialize(request.NewWaypoints);
            _consecutiveDeviations[alert.TravelRouteId] = 0;
        }

        await _unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<TravelRouteDto?>> GetActiveRouteAsync(string userId)
    {
        var route = await _unitOfWork.TravelRoutes.Query()
            .FirstOrDefaultAsync(r => r.UserId == userId && r.Status == RouteStatus.Active);

        return Result<TravelRouteDto?>.Success(route != null ? MapToDto(route) : null);
    }

    public async Task<Result<List<TravelRouteDto>>> GetUserRoutesAsync(string userId)
    {
        var routes = await _unitOfWork.TravelRoutes.Query()
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return Result<List<TravelRouteDto>>.Success(routes.Select(MapToDto).ToList());
    }

    public async Task<Result> TriggerSosAsync(Guid routeId, string userId)
    {
        var route = await _unitOfWork.TravelRoutes.Query()
            .Include(r => r.User)
                .ThenInclude(u => u.RelativeContacts)
            .FirstOrDefaultAsync(r => r.Id == routeId && r.UserId == userId);

        if (route == null) return Result.Failure("Không tìm thấy lộ trình.");

        var lastLog = await _unitOfWork.LocationLogs.Query()
            .Where(l => l.TravelRouteId == routeId)
            .OrderByDescending(l => l.RecordedAt)
            .FirstOrDefaultAsync();

        double lat = (double)(lastLog?.Latitude ?? 0);
        double lng = (double)(lastLog?.Longitude ?? 0);

        var alert = new RouteAlert
        {
            TravelRouteId = routeId,
            AlertType = AlertType.SOS,
            Resolution = AlertResolution.NotifiedRelatives,
            Latitude = (decimal)lat,
            Longitude = (decimal)lng,
            Message = "SOS TÍN HIỆU CỨU HỘ KHẨN CẤP ĐƯỢC KÍCH HOẠT TỪ NGƯỜI DÙNG",
            NotificationsSent = true,
            ResolvedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.RouteAlerts.AddAsync(alert);
        await _unitOfWork.SaveChangesAsync();

        foreach (var rel in route.User.RelativeContacts)
        {
            if (!string.IsNullOrWhiteSpace(rel.Email))
            {
                await _emailService.SendSosAlertAsync(rel.Email, route.User.FullName, lat, lng);
            }
            if (!string.IsNullOrWhiteSpace(rel.TelegramChatId))
            {
                await _telegramService.SendSosAlertAsync(rel.TelegramChatId, route.User.FullName, lat, lng);
            }
        }

        return Result.Ok();
    }

    private static TravelRouteDto MapToDto(TravelRoute r)
    {
        var waypoints = new List<WaypointDto>();
        if (!string.IsNullOrWhiteSpace(r.WaypointsJson))
        {
            try
            {
                waypoints = JsonSerializer.Deserialize<List<WaypointDto>>(r.WaypointsJson) ?? new();
            }
            catch { }
        }

        return new TravelRouteDto
        {
            Id = r.Id,
            UserId = r.UserId,
            RouteName = r.RouteName,
            Status = r.Status,
            Waypoints = waypoints,
            DeviationThresholdMeters = r.DeviationThresholdMeters,
            IsActive = r.IsActive,
            StartedAt = r.StartedAt,
            EndedAt = r.EndedAt,
            Notes = r.Notes
        };
    }
}
