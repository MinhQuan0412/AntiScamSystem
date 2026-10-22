using AntiScam.Domain.Enums;

namespace AntiScam.Application.DTOs.ModelVersion;

/// <summary>
/// DTO representing a trained ML model version and its performance metrics.
/// </summary>
public sealed class ModelVersionDto
{
    /// <summary>Gets or sets the unique identifier of the model version.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the human-readable version label (e.g., "v2.3.1").</summary>
    public string VersionLabel { get; set; } = string.Empty;

    /// <summary>Gets or sets the type of model.</summary>
    public ModelType ModelType { get; set; }

    /// <summary>Gets or sets the human-readable model type name.</summary>
    public string ModelTypeName { get; set; } = string.Empty;

    /// <summary>Gets or sets the classification task this model was trained for.</summary>
    public ClassificationTask ClassificationTask { get; set; }

    /// <summary>Gets or sets accuracy on the validation dataset (0-1).</summary>
    public double Accuracy { get; set; }

    /// <summary>Gets or sets precision on the validation dataset (0-1).</summary>
    public double Precision { get; set; }

    /// <summary>Gets or sets recall on the validation dataset (0-1).</summary>
    public double Recall { get; set; }

    /// <summary>Gets or sets the F1 score (harmonic mean of precision and recall).</summary>
    public double F1Score { get; set; }

    /// <summary>Gets or sets whether this is the currently active (production) model.</summary>
    public bool IsActive { get; set; }

    /// <summary>Gets or sets the number of samples used to train this model version.</summary>
    public int TrainingSampleCount { get; set; }

    /// <summary>Gets or sets the UTC timestamp when training completed.</summary>
    public DateTime TrainedAt { get; set; }

    /// <summary>Gets or sets the identifier of the admin who promoted this version to production.</summary>
    public string? ApprovedByAdminId { get; set; }

    /// <summary>Gets or sets the UTC timestamp when this version was approved.</summary>
    public DateTime? ApprovedAt { get; set; }

    /// <summary>Gets or sets miscellaneous training metadata as a key-value map.</summary>
    public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// DTO representing a model training job and its current status.
/// </summary>
public sealed class TrainingStatusDto
{
    /// <summary>Gets or sets the external job identifier.</summary>
    public string JobId { get; set; } = string.Empty;

    /// <summary>Gets or sets the current status label (e.g., Queued, Running, Completed, Failed).</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Gets or sets a human-readable message describing the current status.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Gets or sets the overall training progress percentage (0-100).</summary>
    public int ProgressPercent { get; set; }

    /// <summary>Gets or sets the UTC timestamp when training started.</summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>Gets or sets the UTC timestamp when training finished.</summary>
    public DateTime? FinishedAt { get; set; }

    /// <summary>Gets or sets the model version produced, if training has completed.</summary>
    public ModelVersionDto? ProducedVersion { get; set; }
}
