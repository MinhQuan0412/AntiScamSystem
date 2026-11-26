using AntiScam.Domain.Enums;

namespace AntiScam.Domain.Entities;

/// <summary>
/// Represents a documented scam scenario used for awareness, training, and detection.
/// Scenarios can form a hierarchy (parent -> child variants) to track how scams evolve over time.
/// </summary>
public class ScamScenario : BaseEntity
{
    /// <summary>
    /// Short, human-readable title for the scenario (e.g., "Bank-Impersonation via OTP Theft").
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The primary category of fraud this scenario falls under.
    /// </summary>
    public ScamType ScamType { get; set; }

    /// <summary>
    /// Detailed explanation of how the scam operates, including the sequence of events
    /// typically used by perpetrators.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Observable signs or red flags that help a potential victim recognise the scam
    /// before being defrauded.
    /// </summary>
    public string RecognitionSigns { get; set; } = string.Empty;

    /// <summary>
    /// A fully anonymised, real-world example of the scenario.
    /// </summary>
    public string AnonymizedExample { get; set; } = string.Empty;

    /// <summary>
    /// Actionable advice explaining how users can protect themselves from this scam.
    /// </summary>
    public string PreventionAdvice { get; set; } = string.Empty;

    /// <summary>
    /// UTC timestamp indicating when this scenario was first documented in the system.
    /// </summary>
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Current lifecycle status of this scenario.
    /// </summary>
    public ScamStatus Status { get; set; } = ScamStatus.Active;

    /// <summary>
    /// User identifier of who created this scenario.
    /// </summary>
    public string? CreatedById { get; set; }

    /// <summary>
    /// Navigation property to creator user.
    /// </summary>
    public new User? CreatedBy { get; set; }

    /// <summary>
    /// Comma-separated tags or keywords.
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// Optional image demonstration URL.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Optional video demonstration URL.
    /// </summary>
    public string? VideoUrl { get; set; }

    // ── Hierarchy ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Foreign key to the parent scenario.
    /// </summary>
    public Guid? ParentScenarioId { get; set; }

    /// <summary>
    /// Navigation property to the parent scenario.
    /// </summary>
    public ScamScenario? ParentScenario { get; set; }

    /// <summary>
    /// Child scenarios / variants.
    /// </summary>
    public ICollection<ScamScenario> RelatedScenarios { get; set; } = new List<ScamScenario>();

    /// <summary>
    /// Alias for RelatedScenarios.
    /// </summary>
    public ICollection<ScamScenario> ChildScenarios => RelatedScenarios;

    // ── Related Data ──────────────────────────────────────────────────────────

    /// <summary>
    /// Labelled training data samples linked to this scenario.
    /// </summary>
    public ICollection<TrainingDataSample> TrainingDataSamples { get; set; } = new List<TrainingDataSample>();
}
