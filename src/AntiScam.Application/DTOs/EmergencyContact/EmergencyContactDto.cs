namespace AntiScam.Application.DTOs.EmergencyContact;

public class EmergencyContactDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? ProvinceCode { get; set; }
    public string? ProvinceName { get; set; }
    public string? Description { get; set; }
    public bool IsNational { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? VerificationSource { get; set; }
}

public class CreateEmergencyContactRequest
{
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? ProvinceCode { get; set; }
    public string? ProvinceName { get; set; }
    public string? Description { get; set; }
    public bool IsNational { get; set; }
    public string? VerificationSource { get; set; }
}
