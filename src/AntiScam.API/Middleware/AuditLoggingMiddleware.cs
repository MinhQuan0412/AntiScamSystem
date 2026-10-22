using System.Security.Claims;
using AntiScam.Domain.Entities;
using AntiScam.Domain.Interfaces;

namespace AntiScam.API.Middleware;

public class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public AuditLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        // Skip logging health checks or swagger
        if (path.StartsWith("/swagger") || path.StartsWith("/health") || context.Request.Method == "OPTIONS")
        {
            await _next(context);
            return;
        }

        var userId = context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var ip = context.Connection.RemoteIpAddress?.ToString();
        var userAgent = context.Request.Headers.UserAgent.ToString();

        await _next(context);

        // Only log modifying actions (POST, PUT, DELETE)
        if (context.Request.Method is "POST" or "PUT" or "DELETE")
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var auditLog = new AuditLog
                {
                    UserId = userId,
                    Action = $"{context.Request.Method} {path} - Status: {context.Response.StatusCode}",
                    EntityName = path.Split('/', StringSplitOptions.RemoveEmptyEntries).Skip(1).FirstOrDefault() ?? "API",
                    IpAddress = ip,
                    UserAgent = userAgent.Length > 490 ? userAgent[..490] : userAgent,
                    CreatedAt = DateTime.UtcNow
                };

                await unitOfWork.AuditLogs.AddAsync(auditLog);
                await unitOfWork.SaveChangesAsync();
            }
            catch
            {
                // Silently avoid breaking the request if audit logging fails
            }
        }
    }
}
