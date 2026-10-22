namespace AntiScam.Application.DTOs.RelativeContact;

public class RelativeContactDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? TelegramChatId { get; set; }
    public string ContactType { get; set; } = "Relative";
    public string? Notes { get; set; }
    public bool IsVerified { get; set; }
}

public class CreateRelativeContactRequest
{
    public string Name { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? TelegramChatId { get; set; }
    public string ContactType { get; set; } = "Relative";
    public string? Notes { get; set; }
}
