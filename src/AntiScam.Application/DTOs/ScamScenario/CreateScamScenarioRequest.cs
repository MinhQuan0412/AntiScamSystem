using System.ComponentModel.DataAnnotations;
using AntiScam.Domain.Enums;

namespace AntiScam.Application.DTOs.ScamScenario;

/// <summary>
/// Request payload for creating a new scam scenario.
/// </summary>
public sealed class CreateScamScenarioRequest
{
    /// <summary>Gets or sets the title of the new scenario.</summary>
    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the category of scam.</summary>
    [Required]
    public ScamType ScamType { get; set; }

    /// <summary>Gets or sets the detailed description of the scam scenario.</summary>
    [Required]
    [MaxLength(5000)]
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets recognition signs that help identify this scam.</summary>
    public List<string> RecognitionSigns { get; set; } = new();

    /// <summary>Gets or sets an anonymized real-world example.</summary>
    [MaxLength(3000)]
    public string? AnonymizedExample { get; set; }

    /// <summary>Gets or sets prevention advice for end users.</summary>
    [Required]
    [MaxLength(3000)]
    public string PreventionAdvice { get; set; } = string.Empty;

    /// <summary>Gets or sets the optional identifier of a parent (master) scenario.</summary>
    public Guid? ParentScenarioId { get; set; }
}
