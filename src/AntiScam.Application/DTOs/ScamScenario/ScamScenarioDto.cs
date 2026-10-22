using AntiScam.Domain.Enums;

namespace AntiScam.Application.DTOs.ScamScenario;

/// <summary>
/// Data Transfer Object representing a scam scenario for client consumption.
/// </summary>
public sealed class ScamScenarioDto
{
    /// <summary>Gets or sets the unique identifier of the scenario.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the scenario title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the category of scam this scenario belongs to.</summary>
    public ScamType ScamType { get; set; }

    /// <summary>Gets or sets the human-readable name of the scam type.</summary>
    public string ScamTypeName { get; set; } = string.Empty;

    /// <summary>Gets or sets the detailed description of this scam scenario.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets a list of recognition signs that identify this scam.</summary>
    public List<string> RecognitionSigns { get; set; } = new();

    /// <summary>Gets or sets an anonymized real-world example of this scam in action.</summary>
    public string? AnonymizedExample { get; set; }

    /// <summary>Gets or sets advice on how to prevent or avoid this type of scam.</summary>
    public string PreventionAdvice { get; set; } = string.Empty;

    /// <summary>Gets or sets the current status of this scenario in the moderation lifecycle.</summary>
    public ScamStatus Status { get; set; }

    /// <summary>Gets or sets the human-readable status name.</summary>
    public string StatusName { get; set; } = string.Empty;

    /// <summary>Gets or sets the identifier of the parent scenario (if this is a variant).</summary>
    public Guid? ParentScenarioId { get; set; }

    /// <summary>Gets or sets the number of child/variant scenarios under this scenario.</summary>
    public int ChildScenariosCount { get; set; }

    /// <summary>Gets or sets the UTC creation timestamp.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Gets or sets the UTC timestamp of the last update.</summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>Gets or sets the identifier of the admin who created this scenario.</summary>
    public string CreatedByAdminId { get; set; } = string.Empty;
}
