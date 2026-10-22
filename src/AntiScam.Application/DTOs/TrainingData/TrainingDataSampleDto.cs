using AntiScam.Domain.Enums;

namespace AntiScam.Application.DTOs.TrainingData;

/// <summary>
/// Data Transfer Object representing a training data sample.
/// </summary>
public sealed class TrainingDataSampleDto
{
    /// <summary>Gets or sets the unique identifier of the sample.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the raw (or anonymized) text content of the sample.</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>Gets or sets the scam label assigned to this sample.</summary>
    public ScamType? ScamType { get; set; }

    /// <summary>Gets or sets the human-readable scam type name.</summary>
    public string? ScamTypeName { get; set; }

    /// <summary>Gets or sets whether this sample has been confirmed as a scam.</summary>
    public bool IsScam { get; set; }

    /// <summary>Gets or sets the classification task this sample targets.</summary>
    public ClassificationTask ClassificationTask { get; set; }

    /// <summary>Gets or sets the current moderation/labelling status.</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Gets or sets the identifier of the user who submitted the sample (anonymized if needed).</summary>
    public string? SubmittedByUserId { get; set; }

    /// <summary>Gets or sets the identifier of the admin who labelled the sample.</summary>
    public string? LabelledByAdminId { get; set; }

    /// <summary>Gets or sets the version of the model used to pre-label this sample.</summary>
    public string? ModelVersionUsed { get; set; }

    /// <summary>Gets or sets the confidence score of the pre-labelling (0-1).</summary>
    public double? PreLabelConfidence { get; set; }

    /// <summary>Gets or sets any labelling notes left by the admin reviewer.</summary>
    public string? LabelNotes { get; set; }

    /// <summary>Gets or sets the UTC creation timestamp.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Gets or sets the UTC timestamp when this sample was labelled.</summary>
    public DateTime? LabelledAt { get; set; }
}
