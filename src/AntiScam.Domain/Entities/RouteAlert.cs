using AntiScam.Domain.Enums;

namespace AntiScam.Domain.Entities;

/// <summary>
/// Alert raised when a safety deviation or SOS occurs during a travel route.
/// </summary>
public class RouteAlert : BaseEntity
{
    public Guid TravelRouteId { get; set; }
    public TravelRoute TravelRoute { get; set; } = null!;

    public AlertType AlertType { get; set; } = AlertType.Deviation;
    public AlertResolution Resolution { get; set; } = AlertResolution.Expired;

    public string? Message { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public int CountdownSeconds { get; set; } = 300;
    public DateTime? AcknowledgedAt { get; set; }
    public string? AcknowledgedById { get; set; }
    public User? AcknowledgedBy { get; set; }

    public bool NotificationsSent { get; set; } = false;
    public DateTime? ResolvedAt { get; set; }
}
