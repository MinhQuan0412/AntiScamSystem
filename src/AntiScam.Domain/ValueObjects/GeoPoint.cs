namespace AntiScam.Domain.ValueObjects;

/// <summary>
/// Geographic coordinate point with Haversine distance calculations.
/// </summary>
public readonly record struct GeoPoint(double Latitude, double Longitude)
{
    private const double EarthRadiusMeters = 6371000.0;

    /// <summary>
    /// Calculates distance in meters to another GeoPoint using the Haversine formula.
    /// </summary>
    public double DistanceTo(GeoPoint other)
    {
        double dLat = ToRadians(other.Latitude - Latitude);
        double dLon = ToRadians(other.Longitude - Longitude);

        double lat1 = ToRadians(Latitude);
        double lat2 = ToRadians(other.Latitude);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusMeters * c;
    }

    private static double ToRadians(double degrees) => degrees * (Math.PI / 180.0);
}
