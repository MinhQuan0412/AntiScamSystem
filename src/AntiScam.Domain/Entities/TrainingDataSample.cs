using AntiScam.Domain.Enums;

namespace AntiScam.Domain.Entities;

/// <summary>
/// A single labelled data point used for training or evaluating scam-detection models.
/// All content must be fully anonymised before being stored.
/// </summary>
public class TrainingDataSample : BaseEntity
{
    /// <summary>
    /// The raw text content of the message, conversation, or situation being labelled.
    /// Must be fully anonymised (no real names, phone numbers, or account details).
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Ground-truth binary label: true if scam, false if legitimate.
    /// </summary>
    public bool IsScam { get; set; }

    /// <summary>
    /// Fine-grained scam category label.
    /// </summary>
    public ScamType? ScamType { get; set; }

    /// <summary>
    /// Identifies which data batch this sample belongs to (e.g. "old", "new").
    /// </summary>
    public string BatchId { get; set; } = "batch-1";

    /// <summary>
    /// Alias for BatchId for backward compatibility.
    /// </summary>
    public string DataBatch
    {
        get => BatchId;
        set => BatchId = value;
    }

    /// <summary>
    /// Human-readable description of where this sample originated.
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Admin user identifier who labeled/verified this sample.
    /// </summary>
    public string? LabeledById { get; set; }

    /// <summary>
    /// User who labeled/verified this sample.
    /// </summary>
    public User? LabeledBy { get; set; }

    /// <summary>
    /// Username or identifier of the admin who reviewed and verified this sample.
    /// </summary>
    public string? VerifiedBy
    {
        get => LabeledById;
        set => LabeledById = value;
    }

    /// <summary>
    /// UTC timestamp when the sample was verified.
    /// </summary>
    public DateTime? VerifiedAt { get; set; }

    // ── Scenario Link ─────────────────────────────────────────────────────────

    /// <summary>
    /// Optional foreign key linking this sample to a known <see cref="ScamScenario"/>.
    /// </summary>
    public Guid? ScenarioId { get; set; }

    /// <summary>
    /// Navigation property to the associated scenario.
    /// </summary>
    public ScamScenario? Scenario { get; set; }

    /// <summary>
    /// Indicates whether this sample is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}
