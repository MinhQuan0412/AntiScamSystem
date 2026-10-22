using AntiScam.Application.DTOs.TravelRoute;
using AntiScam.Domain.Enums;

namespace AntiScam.Application.DTOs.Alert;

public class ReportLocationRequest
{
    public Guid TravelRouteId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? Accuracy { get; set; }
    public double? Speed { get; set; }
    public double? Bearing { get; set; }
}

public enum AlertActionResponse
{
    ImOkay = 0,
    ChangeRoute = 1
}

public class AlertResponseRequest
{
    public Guid AlertId { get; set; }
    public AlertActionResponse Action { get; set; }
    public List<WaypointDto>? NewWaypoints { get; set; }
}

public class RouteAlertDto
{
    public Guid Id { get; set; }
    public Guid TravelRouteId { get; set; }
    public AlertType AlertType { get; set; }
    public AlertResolution Resolution { get; set; }
    public string? Message { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int CountdownSeconds { get; set; }
    public bool NotificationsSent { get; set; }
    public DateTime CreatedAt { get; set; }
}
