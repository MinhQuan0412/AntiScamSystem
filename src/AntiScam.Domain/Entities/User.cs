using AntiScam.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace AntiScam.Domain.Entities;

/// <summary>
/// Represents a user account in the Anti-Scam platform.
/// Extends ASP.NET Core Identity's <see cref="IdentityUser"/>
/// with application-specific profile and relationship properties.
/// </summary>
public class User : IdentityUser
{
    // ── Profile ──────────────────────────────────────────────────────────────

    /// <summary>
    /// The user's full display name (e.g., "Nguyễn Văn A").
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Optional date of birth used for age-related scam warnings.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Optional residential or mailing address.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Optional URL pointing to the user's avatar / profile picture.
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Telegram chat ID for alert notifications.
    /// </summary>
    public string? TelegramChatId { get; set; }

    // ── Status ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Indicates whether the account is active and allowed to log in.
    /// Administrators can deactivate accounts without deleting them.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// UTC timestamp of account creation.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// UTC timestamp of the user's most recent successful login.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Application-level role of the user.
    /// </summary>
    public UserRole Role { get; set; } = UserRole.User;

    // ── Navigation Properties ─────────────────────────────────────────────────

    /// <summary>
    /// Trusted relative or friend contacts registered by this user
    /// to be notified in case of a safety alert.
    /// </summary>
    public ICollection<RelativeContact> RelativeContacts { get; set; } = new List<RelativeContact>();

    /// <summary>
    /// Travel routes being tracked for this user's safety.
    /// </summary>
    public ICollection<TravelRoute> TravelRoutes { get; set; } = new List<TravelRoute>();

    /// <summary>
    /// Scam phone/bank-account reports submitted by this user.
    /// </summary>
    public ICollection<PhoneReport> PhoneReports { get; set; } = new List<PhoneReport>();
}
