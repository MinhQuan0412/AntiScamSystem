using AntiScam.Domain.Entities;
using AntiScam.Domain.Enums;
using AntiScam.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AntiScam.Infrastructure.BackgroundServices;

/// <summary>
/// Nightly cleanup job that purges detailed GPS logs for completed/cancelled trips
/// to guarantee user privacy.
/// </summary>
public class LocationCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LocationCleanupService> _logger;

    public LocationCleanupService(IServiceProvider serviceProvider, ILogger<LocationCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var cutoff = DateTime.UtcNow.AddHours(-24);

                var expiredRouteIds = await unitOfWork.TravelRoutes.Query()
                    .Where(r => (r.Status == RouteStatus.Completed || r.Status == RouteStatus.Cancelled) &&
                                r.EndedAt.HasValue && r.EndedAt < cutoff)
                    .Select(r => r.Id)
                    .ToListAsync(stoppingToken);

                if (expiredRouteIds.Count > 0)
                {
                    var oldLogs = await unitOfWork.LocationLogs.Query()
                        .Where(l => expiredRouteIds.Contains(l.TravelRouteId))
                        .ToListAsync(stoppingToken);

                    foreach (var log in oldLogs)
                    {
                        unitOfWork.LocationLogs.Delete(log);
                    }

                    await unitOfWork.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("Purged {Count} historical GPS location logs for {RoutesCount} ended travel routes.", oldLogs.Count, expiredRouteIds.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during daily GPS location logs cleanup.");
            }

            // Run once every 24 hours
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
