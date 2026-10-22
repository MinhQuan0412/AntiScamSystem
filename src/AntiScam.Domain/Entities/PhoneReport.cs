using AntiScam.Domain.Enums;

namespace AntiScam.Domain.Entities;

/// <summary>
/// Aggregated record of scam reports targeting a specific phone number
/// or bank account number.
/// </summary>
public class PhoneReport : BaseEntity
{
    /// <summary>
    /// The reported phone or bank account number.
    /// </summary>
    public string ReportedNumber { get; set; } = string.Empty;

    /// <summary>
    /// Whether this entry represents a phone number or bank account.
    /// </summary>
    public NumberType NumberType { get; set; }

    /// <summary>
    /// Optional identified owner or caller ID.
    /// </summary>
    public string? OwnerName { get; set; }

    /// <summary>
    /// Organization or bank name.
    /// </summary>
    public string? OwnerOrganization { get; set; }

    /// <summary>
    /// Bank name alias.
    /// </summary>
    public string? BankName
    {
        get => OwnerOrganization;
        set => OwnerOrganization = value;
    }

    /// <summary>
    /// Primary scam type observed for this number.
    /// </summary>
    public ScamType? PrimaryScamType { get; set; }

    // ── Reporting Statistics ──────────────────────────────────────────────────

    public int ReportCount { get; set; } = 0;
    public int UniqueReporterCount { get; set; } = 0;
    public DateTime FirstSeenAt { get; set; } = DateTime.UtcNow;

    // ── Moderation ────────────────────────────────────────────────────────────

    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Unverified;
    public string? VerifiedById { get; set; }
    public User? VerifiedBy { get; set; }

    // ── Risk Assessment ───────────────────────────────────────────────────────

    public double RiskScore { get; set; } = 0.0;
    public string RiskFactors { get; set; } = "{}";
    public string Status { get; set; } = "NeedsAttention";
    public string? Tags { get; set; }

    // ── Navigation Properties ─────────────────────────────────────────────────

    /// <summary>
    /// Individual detailed reports.
    /// </summary>
    public ICollection<PhoneReportDetail> Details { get; set; } = new List<PhoneReportDetail>();

    public ICollection<PhoneReportDetail> Reports => Details;
}
