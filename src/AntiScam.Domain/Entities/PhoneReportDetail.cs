using AntiScam.Domain.Enums;

namespace AntiScam.Domain.Entities;

/// <summary>
/// Individual user submission detailing a fraudulent phone or bank account incident.
/// </summary>
public class PhoneReportDetail : BaseEntity
{
    public Guid PhoneReportId { get; set; }
    public PhoneReport PhoneReport { get; set; } = null!;

    public string ReportedById { get; set; } = string.Empty;
    public User ReportedBy { get; set; } = null!;

    public string? Description { get; set; }
    public ScamType? ScamType { get; set; }
    public string? EvidenceUrl { get; set; }

    public bool IsModerated { get; set; } = false;
    public string? ModeratedById { get; set; }
    public User? ModeratedBy { get; set; }
    public DateTime? ModeratedAt { get; set; }
    public bool IsApproved { get; set; } = false;
    public string? ModerationNote { get; set; }
}
