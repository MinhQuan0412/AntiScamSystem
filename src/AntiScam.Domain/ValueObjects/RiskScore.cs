using AntiScam.Domain.Enums;

namespace AntiScam.Domain.ValueObjects;

/// <summary>
/// Calculated scam risk evaluation result.
/// </summary>
public sealed record RiskScore(
    double Score,
    Dictionary<string, double> Factors,
    RiskLevel Level,
    string Explanation)
{
    public static RiskScore Create(double score, Dictionary<string, double> factors, string explanation)
    {
        double clamped = Math.Clamp(score, 0.0, 100.0);
        RiskLevel level = clamped switch
        {
            < 25.0 => RiskLevel.Low,
            < 50.0 => RiskLevel.Medium,
            < 75.0 => RiskLevel.High,
            _ => RiskLevel.Critical
        };

        return new RiskScore(clamped, factors, level, explanation);
    }
}
