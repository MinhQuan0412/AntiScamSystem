using AntiScam.Domain.Enums;

namespace AntiScam.Application.DTOs.PhoneReport;

public class RiskScoreDto
{
    public double Score { get; set; }
    public RiskLevel Level { get; set; }
    public string LevelName => Level.ToString();
    public string Explanation { get; set; } = string.Empty;
    public Dictionary<string, double> Factors { get; set; } = new();
}

public class PhoneReportDetailDto
{
    public Guid Id { get; set; }
    public Guid PhoneReportId { get; set; }
    public string ReportedById { get; set; } = string.Empty;
    public string ReporterName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ScamType? ScamType { get; set; }
    public string? EvidenceUrl { get; set; }
    public bool IsModerated { get; set; }
    public bool IsApproved { get; set; }
    public string? ModerationNote { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PhoneReportDto
{
    public Guid Id { get; set; }
    public string ReportedNumber { get; set; } = string.Empty;
    public NumberType NumberType { get; set; }
    public string? OwnerName { get; set; }
    public string? BankName { get; set; }
    public int ReportCount { get; set; }
    public int UniqueReporterCount { get; set; }
    public DateTime FirstSeenAt { get; set; }
    public VerificationStatus VerificationStatus { get; set; }
    public double RiskScore { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class CreatePhoneReportRequest
{
    public string ReportedNumber { get; set; } = string.Empty;
    public NumberType NumberType { get; set; } = NumberType.Phone;
    public string? BankName { get; set; }
    public ScamType? ScamType { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? EvidenceUrl { get; set; }
}

public class PhoneReportLookupResponse
{
    public string MaskedNumber { get; set; } = string.Empty;
    public NumberType NumberType { get; set; }
    public string? BankName { get; set; }
    public int ReportCount { get; set; }
    public int UniqueReporterCount { get; set; }
    public DateTime FirstSeenAt { get; set; }
    public VerificationStatus VerificationStatus { get; set; }
    public RiskScoreDto RiskScore { get; set; } = new();
    public bool NeedsAttention { get; set; }
    public List<PhoneReportDetailDto> RecentReports { get; set; } = new();
}
