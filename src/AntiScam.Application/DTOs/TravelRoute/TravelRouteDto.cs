using AntiScam.Domain.Enums;

namespace AntiScam.Application.DTOs.TravelRoute;

public class WaypointDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Name { get; set; }
    public int Order { get; set; }
}

public class CreateTravelRouteRequest
{
    public string RouteName { get; set; } = string.Empty;
    public List<WaypointDto> Waypoints { get; set; } = new();
    public double DeviationThresholdMeters { get; set; } = 200.0;
    public string? Notes { get; set; }
}

public class TravelRouteDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string RouteName { get; set; } = string.Empty;
    public RouteStatus Status { get; set; }
    public List<WaypointDto> Waypoints { get; set; } = new();
    public double DeviationThresholdMeters { get; set; }
    public bool IsActive { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public string? Notes { get; set; }
}

public class LocationReportResult
{
    public bool IsDeviating { get; set; }
    public double CurrentDistanceMeters { get; set; }
    public Guid? AlertId { get; set; }
    public int ConsecutiveDeviationCount { get; set; }
    public string Message { get; set; } = string.Empty;
}
