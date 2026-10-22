namespace AntiScam.Application.DTOs.Auth;

/// <summary>
/// Returned by the authentication endpoint upon a successful login.
/// </summary>
public sealed class LoginResponse
{
    /// <summary>Gets or sets the short-lived JWT access token.</summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>Gets or sets the opaque refresh token used to obtain a new access token.</summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>Gets or sets the UTC date-time at which the access token expires.</summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>Gets or sets the unique identifier of the authenticated user.</summary>
    public Guid UserId { get; set; }

    /// <summary>Gets or sets the full display name of the authenticated user.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Gets or sets the email address of the authenticated user.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the role assigned to the authenticated user (e.g. Admin, User).</summary>
    public string Role { get; set; } = string.Empty;
}
