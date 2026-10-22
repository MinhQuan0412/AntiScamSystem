using System.ComponentModel.DataAnnotations;

namespace AntiScam.Application.DTOs.Auth;

/// <summary>
/// Payload sent by a client to authenticate with the system.
/// </summary>
public sealed class LoginRequest
{
    /// <summary>Gets or sets the user's registered email address.</summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's password.</summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}
