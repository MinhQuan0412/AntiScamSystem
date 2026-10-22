using AntiScam.Domain.Entities;
using AntiScam.Domain.Enums;
using AntiScam.Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace AntiScam.UnitTests.Services;

public class RiskScoreCalculatorTests
{
    private readonly RiskScoreCalculator _calculator;

    public RiskScoreCalculatorTests()
    {
        _calculator = new RiskScoreCalculator();
    }

    [Fact]
    public void Calculate_ZeroReports_ReturnsLowRisk()
    {
        var report = new PhoneReport
        {
            ReportCount = 0,
            UniqueReporterCount = 0,
            VerificationStatus = VerificationStatus.Unverified,
            FirstSeenAt = DateTime.UtcNow
        };

        var score = _calculator.Calculate(report);

        score.Score.Should().BeLessThan(25.0);
        score.Level.Should().Be(RiskLevel.Low);
    }

    [Fact]
    public void Calculate_MultipleReportersAndVerified_ReturnsCriticalRisk()
    {
        var report = new PhoneReport
        {
            ReportCount = 50,
            UniqueReporterCount = 10,
            VerificationStatus = VerificationStatus.Verified,
            PrimaryScamType = ScamType.BankImpersonation,
            FirstSeenAt = DateTime.UtcNow
        };

        var score = _calculator.Calculate(report);

        score.Score.Should().BeGreaterThanOrEqualTo(75.0);
        score.Level.Should().Be(RiskLevel.Critical);
    }

    [Fact]
    public void Calculate_FalseReport_ReducesRiskScore()
    {
        var report = new PhoneReport
        {
            ReportCount = 2,
            UniqueReporterCount = 1,
            VerificationStatus = VerificationStatus.FalseReport,
            FirstSeenAt = DateTime.UtcNow
        };

        var score = _calculator.Calculate(report);

        score.Score.Should().Be(0.0);
        score.Level.Should().Be(RiskLevel.Low);
    }

    [Fact]
    public void Calculate_NeverExceeds100()
    {
        var report = new PhoneReport
        {
            ReportCount = 10000,
            UniqueReporterCount = 1000,
            VerificationStatus = VerificationStatus.Verified,
            PrimaryScamType = ScamType.BankImpersonation,
            FirstSeenAt = DateTime.UtcNow
        };

        var score = _calculator.Calculate(report);

        score.Score.Should().BeLessOrEqualTo(100.0);
    }
}
