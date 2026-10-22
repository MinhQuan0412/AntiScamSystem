namespace AntiScam.Domain.Entities;

/// <summary>
/// Audit trail of user and system actions for security, compliance and traceability.
/// </summary>
public class AuditLog
{
    public long Id { get; set; }
    public string? UserId { get; set; }
    public User? User { get; set; }

    public string Action { get; set; } = string.Empty;
    public string? EntityName { get; set; }
    public string? EntityId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
