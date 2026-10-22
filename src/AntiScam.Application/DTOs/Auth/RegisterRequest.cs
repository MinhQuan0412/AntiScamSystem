using System.ComponentModel.DataAnnotations;

namespace AntiScam.Application.DTOs.Auth;

/// <summary>
/// Payload sent by a client to register a new user account.
/// </summary>
public sealed class RegisterRequest
{
    /// <summary>Gets or sets the desired email address (used as the login username).</summary>
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the desired password.</summary>
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    /// <summary>Gets or sets the password confirmation; must match <see cref="Password"/>.</summary>
    [Required]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's full display name.</summary>
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    /// <summary>Gets or sets an optional contact phone number.</summary>
    [Phone]
    [MaxLength(20)]
    public string? Phone { get; set; }
}
