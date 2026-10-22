using System.Text.Json;
using AntiScam.Domain.Entities;
using AntiScam.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace AntiScam.Infrastructure.Services;

public record WaypointData(double Latitude, double Longitude, string? Name, int Order);

public interface IRouteDeviationDetector
{
    bool IsDeviating(GeoPoint current, TravelRoute route, int consecutiveReadings);
    double GetDistanceToRouteMeters(GeoPoint current, TravelRoute route);
    double DistanceToSegment(GeoPoint p, GeoPoint a, GeoPoint b);
}

public class RouteDeviationDetector : IRouteDeviationDetector
{
    private readonly ILogger<RouteDeviationDetector> _logger;

    public RouteDeviationDetector(ILogger<RouteDeviationDetector> logger)
    {
        _logger = logger;
    }

    public bool IsDeviating(GeoPoint current, TravelRoute route, int consecutiveReadings)
    {
        double distance = GetDistanceToRouteMeters(current, route);
        bool exceeded = distance > route.DeviationThresholdMeters;

        // Anti-jitter GPS rule: only trigger deviation if reading exceeded for 3 or more consecutive readings
        return exceeded && consecutiveReadings >= 3;
    }

    public double GetDistanceToRouteMeters(GeoPoint current, TravelRoute route)
    {
        var waypoints = ParseWaypoints(route.WaypointsJson);
        if (waypoints.Count == 0) return 0.0;
        if (waypoints.Count == 1)
        {
            var single = new GeoPoint(waypoints[0].Latitude, waypoints[0].Longitude);
            return current.DistanceTo(single);
        }

        double minDistance = double.MaxValue;

        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            var p1 = new GeoPoint(waypoints[i].Latitude, waypoints[i].Longitude);
            var p2 = new GeoPoint(waypoints[i + 1].Latitude, waypoints[i + 1].Longitude);

            double dist = DistanceToSegment(current, p1, p2);
            if (dist < minDistance)
            {
                minDistance = dist;
            }
        }

        return minDistance;
    }

    public double DistanceToSegment(GeoPoint p, GeoPoint a, GeoPoint b)
    {
        // Vector projection in equirectangular approximation
        double x1 = a.Longitude, y1 = a.Latitude;
        double x2 = b.Longitude, y2 = b.Latitude;
        double px = p.Longitude, py = p.Latitude;

        double dx = x2 - x1;
        double dy = y2 - y1;

        if (Math.Abs(dx) < 1e-9 && Math.Abs(dy) < 1e-9)
        {
            return p.DistanceTo(a);
        }

        // Projection factor t
        double t = ((px - x1) * dx + (py - y1) * dy) / (dx * dx + dy * dy);
        t = Math.Clamp(t, 0.0, 1.0);

        var nearest = new GeoPoint(y1 + t * dy, x1 + t * dx);
        return p.DistanceTo(nearest);
    }

    private List<WaypointData> ParseWaypoints(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new List<WaypointData>();
        try
        {
            return JsonSerializer.Deserialize<List<WaypointData>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new List<WaypointData>();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse waypoints JSON");
            return new List<WaypointData>();
        }
    }
}
