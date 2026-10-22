namespace AntiScam.Domain.Entities;

/// <summary>
/// A trusted guardian/relative registered to receive SOS and emergency alerts.
/// </summary>
public class RelativeContact : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Phone
    {
        get => PhoneNumber;
        set => PhoneNumber = value;
    }

    public string? Email { get; set; }
    public string? TelegramChatId { get; set; }
    public string ContactType { get; set; } = "Relative";
    public string? Notes { get; set; }
    public bool IsVerified { get; set; } = true;
    public DateTime? VerifiedAt { get; set; } = DateTime.UtcNow;
}
