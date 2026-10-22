using AntiScam.Domain.Enums;

namespace AntiScam.Domain.Entities;

/// <summary>
/// A travel safety route monitored by the system.
/// </summary>
public class TravelRoute : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;

    public string? Name { get; set; }
    public string RouteName
    {
        get => Name ?? string.Empty;
        set => Name = value;
    }

    public RouteStatus Status { get; set; } = RouteStatus.Planned;
    public string? WaypointsJson { get; set; }
    public string Waypoints
    {
        get => WaypointsJson ?? "[]";
        set => WaypointsJson = value;
    }

    public double DeviationThresholdMeters { get; set; } = 200.0;
    public bool IsActive { get; set; } = true;
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public string? Notes { get; set; }

    public ICollection<LocationLog> LocationLogs { get; set; } = new List<LocationLog>();
    public ICollection<RouteAlert> Alerts { get; set; } = new List<RouteAlert>();
}
