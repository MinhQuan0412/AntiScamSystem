using System.ComponentModel.DataAnnotations;
using AntiScam.Domain.Enums;

namespace AntiScam.Application.DTOs.ScamScenario;

/// <summary>
/// Request payload for updating an existing scam scenario.
/// </summary>
public sealed class UpdateScamScenarioRequest
{
    /// <summary>Gets or sets the updated title.</summary>
    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the updated scam type category.</summary>
    [Required]
    public ScamType ScamType { get; set; }

    /// <summary>Gets or sets the updated description.</summary>
    [Required]
    [MaxLength(5000)]
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets the updated recognition signs.</summary>
    public List<string> RecognitionSigns { get; set; } = new();

    /// <summary>Gets or sets the updated anonymized example.</summary>
    [MaxLength(3000)]
    public string? AnonymizedExample { get; set; }

    /// <summary>Gets or sets the updated prevention advice.</summary>
    [Required]
    [MaxLength(3000)]
    public string PreventionAdvice { get; set; } = string.Empty;

    /// <summary>Gets or sets the optional parent scenario identifier.</summary>
    public Guid? ParentScenarioId { get; set; }

    /// <summary>Gets or sets the publication/moderation status of this scenario.</summary>
    [Required]
    public ScamStatus Status { get; set; }
}
