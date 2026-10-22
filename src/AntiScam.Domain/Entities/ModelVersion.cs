using AntiScam.Domain.Enums;

namespace AntiScam.Domain.Entities;

/// <summary>
/// Represents a trained machine-learning model version used for scam detection.
/// </summary>
public class ModelVersion : BaseEntity
{
    /// <summary>
    /// Human-readable version tag (e.g. "v1.0", "v1.1-nb").
    /// </summary>
    public string VersionTag { get; set; } = string.Empty;

    /// <summary>
    /// Alias for VersionTag.
    /// </summary>
    public string VersionNumber
    {
        get => VersionTag;
        set => VersionTag = value;
    }

    /// <summary>
    /// Description of the model version.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The machine-learning algorithm used.
    /// </summary>
    public ModelType ModelType { get; set; }

    /// <summary>
    /// The NLP classification task this model is trained to perform.
    /// </summary>
    public ClassificationTask TaskType { get; set; } = ClassificationTask.BinaryDetection;

    // ── Evaluation Metrics ────────────────────────────────────────────────────

    public double Accuracy { get; set; }
    public double Precision { get; set; }
    public double Recall { get; set; }
    public double F1Score { get; set; }

    /// <summary>
    /// JSON-serialized confusion matrix or detailed metrics.
    /// </summary>
    public string Metrics { get; set; } = "{}";

    /// <summary>
    /// JSON-serialized confusion matrix alias.
    /// </summary>
    public string ConfusionMatrix
    {
        get => Metrics;
        set => Metrics = value;
    }

    /// <summary>
    /// JSON-serialized hyperparameters.
    /// </summary>
    public string Hyperparameters { get; set; } = "{}";

    // ── Training Metadata ─────────────────────────────────────────────────────

    public int TrainingSamples { get; set; }
    public int TestSamples { get; set; }
    public string DataBatch { get; set; } = string.Empty;
    public DateTime TrainedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// File path to model artifact.
    /// </summary>
    public string ArtifactPath { get; set; } = string.Empty;

    public string ModelFilePath
    {
        get => ArtifactPath;
        set => ArtifactPath = value;
    }

    public string? TrainedById { get; set; }
    public User? TrainedBy { get; set; }

    /// <summary>
    /// Whether this is the active model version in production.
    /// </summary>
    public bool IsActive { get; set; } = false;

    public string? Notes { get; set; }
}
