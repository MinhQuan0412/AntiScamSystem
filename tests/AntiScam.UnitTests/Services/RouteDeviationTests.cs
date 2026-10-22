using AntiScam.Domain.Entities;
using AntiScam.Domain.ValueObjects;
using AntiScam.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AntiScam.UnitTests.Services;

public class RouteDeviationTests
{
    private readonly RouteDeviationDetector _detector;

    public RouteDeviationTests()
    {
        _detector = new RouteDeviationDetector(NullLogger<RouteDeviationDetector>.Instance);
    }

    [Fact]
    public void HaversineDistance_SamePoint_ReturnsZero()
    {
        var p1 = new GeoPoint(21.0285, 105.8542); // Hanoi
        var p2 = new GeoPoint(21.0285, 105.8542);

        double distance = p1.DistanceTo(p2);

        distance.Should().BeApproximately(0.0, 0.01);
    }

    [Fact]
    public void HaversineDistance_KnownCoordinates_ReturnsAccurateMeters()
    {
        // Hoan Kiem Lake to Hanoi Opera House (~800m)
        var p1 = new GeoPoint(21.0285, 105.8542);
        var p2 = new GeoPoint(21.0245, 105.8575);

        double distance = p1.DistanceTo(p2);

        distance.Should().BeInRange(400.0, 700.0);
    }

    [Fact]
    public void IsDeviating_LessThanThreshold_ReturnsFalse()
    {
        var route = new TravelRoute
        {
            WaypointsJson = "[{\"Latitude\": 21.0285, \"Longitude\": 105.8542, \"Order\": 1}, {\"Latitude\": 21.0295, \"Longitude\": 105.8552, \"Order\": 2}]",
            DeviationThresholdMeters = 200.0
        };

        // Point is right near the start
        var current = new GeoPoint(21.02851, 105.85421);

        bool deviating = _detector.IsDeviating(current, route, consecutiveReadings: 5);

        deviating.Should().BeFalse();
    }

    [Fact]
    public void IsDeviating_ExceedsThreshold_ButConsecutiveLessThan3_ReturnsFalse_DueToAntiJitter()
    {
        var route = new TravelRoute
        {
            WaypointsJson = "[{\"Latitude\": 21.0285, \"Longitude\": 105.8542, \"Order\": 1}, {\"Latitude\": 21.0295, \"Longitude\": 105.8552, \"Order\": 2}]",
            DeviationThresholdMeters = 100.0
        };

        // Far away point (~10km)
        var current = new GeoPoint(21.1000, 105.9000);

        // Even though distance > threshold, only 2 consecutive readings means it does not trigger alert yet
        bool deviating = _detector.IsDeviating(current, route, consecutiveReadings: 2);

        deviating.Should().BeFalse();
    }

    [Fact]
    public void IsDeviating_ExceedsThreshold_AndConsecutiveAtLeast3_ReturnsTrue()
    {
        var route = new TravelRoute
        {
            WaypointsJson = "[{\"Latitude\": 21.0285, \"Longitude\": 105.8542, \"Order\": 1}, {\"Latitude\": 21.0295, \"Longitude\": 105.8552, \"Order\": 2}]",
            DeviationThresholdMeters = 100.0
        };

        var current = new GeoPoint(21.1000, 105.9000);

        bool deviating = _detector.IsDeviating(current, route, consecutiveReadings: 3);

        deviating.Should().BeTrue();
    }
}
