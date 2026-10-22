namespace AntiScam.Domain.Entities;

/// <summary>
/// National and provincial emergency hotlines and authorities.
/// </summary>
public class EmergencyContact : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? ProvinceCode { get; set; }
    public string? ProvinceName { get; set; }
    public string? Description { get; set; }
    public bool IsNational { get; set; } = false;
    public DateTime? VerifiedAt { get; set; }
    public string? VerificationSource { get; set; }
}
