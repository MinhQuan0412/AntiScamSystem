namespace AntiScam.Application.DTOs.ModelVersion;

public class TrainingStatusDto
{
    public string JobId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ModelType { get; set; }
    public int Progress { get; set; }
    public string? Error { get; set; }
}
