using System.Collections.Concurrent;
using AntiScam.Domain.Entities;
using AntiScam.Domain.Enums;
using AntiScam.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AntiScam.Infrastructure.BackgroundServices;

public interface IAlertCountdownService
{
    void StartCountdown(Guid alertId, int seconds);
    void CancelCountdown(Guid alertId);
}

public class AlertCountdownService : BackgroundService, IAlertCountdownService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AlertCountdownService> _logger;
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _countdowns = new();

    public AlertCountdownService(IServiceProvider serviceProvider, ILogger<AlertCountdownService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public void StartCountdown(Guid alertId, int seconds)
    {
        CancelCountdown(alertId);

        var cts = new CancellationTokenSource();
        if (_countdowns.TryAdd(alertId, cts))
        {
            _logger.LogInformation("Starting deviation alert countdown for AlertId {AlertId} with duration {Seconds}s", alertId, seconds);
            _ = RunCountdownTimerAsync(alertId, seconds, cts.Token);
        }
    }

    public void CancelCountdown(Guid alertId)
    {
        if (_countdowns.TryRemove(alertId, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
            _logger.LogInformation("Cancelled alert countdown for AlertId {AlertId}", alertId);
        }
    }

    private async Task RunCountdownTimerAsync(Guid alertId, int seconds, CancellationToken token)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds), token);

            if (token.IsCancellationRequested) return;

            _logger.LogWarning("Alert countdown expired for AlertId {AlertId}. Triggering automated relative notification!", alertId);

            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var emailService = scope.ServiceProvider.GetRequiredService<Services.IEmailService>();
            var telegramService = scope.ServiceProvider.GetRequiredService<Services.ITelegramService>();

            var alert = await unitOfWork.RouteAlerts.Query()
                .Include(a => a.TravelRoute)
                    .ThenInclude(r => r.User)
                        .ThenInclude(u => u.RelativeContacts)
                .FirstOrDefaultAsync(a => a.Id == alertId);

            if (alert != null && alert.Resolution != AlertResolution.UserConfirmedSafe && alert.Resolution != AlertResolution.UserChangedRoute)
            {
                alert.Resolution = AlertResolution.NotifiedRelatives;
                alert.NotificationsSent = true;
                alert.ResolvedAt = DateTime.UtcNow;

                var user = alert.TravelRoute.User;
                double lat = (double)(alert.Latitude ?? 0);
                double lng = (double)(alert.Longitude ?? 0);

                foreach (var relative in user.RelativeContacts)
                {
                    if (!string.IsNullOrWhiteSpace(relative.Email))
                    {
                        await emailService.SendAlertToRelativeAsync(
                            relative.Email,
                            user.FullName,
                            lat,
                            lng,
                            alert.CreatedAt,
                            alert.Message ?? "Người dùng đi lệch lộ trình và không phản hồi chuông cảnh báo.");
                    }

                    if (!string.IsNullOrWhiteSpace(relative.TelegramChatId))
                    {
                        await telegramService.SendAlertToRelativeAsync(
                            relative.TelegramChatId,
                            user.FullName,
                            lat,
                            lng,
                            alert.CreatedAt,
                            alert.Message ?? "Lệch lộ trình quá ngưỡng an toàn.");
                    }
                }

                await unitOfWork.SaveChangesAsync();
            }
        }
        catch (TaskCanceledException)
        {
            // Expected when user confirms safe or cancels
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing countdown expiry for AlertId {AlertId}", alertId);
        }
        finally
        {
            _countdowns.TryRemove(alertId, out _);
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // BackgroundService daemon
        return Task.CompletedTask;
    }
}
