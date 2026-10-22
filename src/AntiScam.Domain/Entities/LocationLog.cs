namespace AntiScam.Domain.Entities;

/// <summary>
/// Recorded GPS location log along a user's travel route.
/// </summary>
public class LocationLog : BaseEntity
{
    public Guid TravelRouteId { get; set; }
    public TravelRoute TravelRoute { get; set; } = null!;

    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal? Accuracy { get; set; }
    public decimal? Speed { get; set; }
    public decimal? Bearing { get; set; }

    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    public double DistanceFromRoute { get; set; }
    public bool IsDeviation { get; set; }
}
