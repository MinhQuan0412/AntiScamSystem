namespace AntiScam.Domain.Enums;

// ─────────────────────────────────────────────────────────────────────────────
// User / Identity
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Application-level roles assigned to a user account.
/// </summary>
public enum UserRole
{
    /// <summary>Regular end-user of the platform.</summary>
    User = 0,

    /// <summary>Platform administrator with elevated privileges.</summary>
    Admin = 1
}

// ─────────────────────────────────────────────────────────────────────────────
// Scam Scenario
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Lifecycle status of a scam scenario entry.
/// </summary>
public enum ScamStatus
{
    /// <summary>The scenario is currently being actively observed.</summary>
    Active = 0,

    /// <summary>The scenario has evolved into a new variant; may still occur.</summary>
    Evolved = 1,

    /// <summary>The scenario is no longer active and has been archived for reference.</summary>
    Archived = 2
}

// ─────────────────────────────────────────────────────────────────────────────
// ML / Model
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Machine-learning algorithm type used to train a model version.
/// </summary>
public enum ModelType
{
    /// <summary>Probabilistic classifier based on Bayes' theorem (library-based).</summary>
    NaiveBayes = 0,

    /// <summary>Linear model for binary/multi-class classification.</summary>
    LogisticRegression = 1,

    /// <summary>Support Vector Machine classifier.</summary>
    SVM = 2,

    /// <summary>Ensemble of decision trees.</summary>
    RandomForest = 3,

    /// <summary>Single decision-tree classifier.</summary>
    DecisionTree = 4,

    /// <summary>Hand-crafted Naive Bayes implementation for experiment comparison.</summary>
    ManualNaiveBayes = 5
}

/// <summary>
/// The specific NLP/classification task the model is trained to perform.
/// </summary>
public enum ClassificationTask
{
    /// <summary>Binary: determines whether a message is scam or not scam.</summary>
    BinaryDetection = 0,

    /// <summary>Multi-class: determines the type of scam (maps to <see cref="ScamType"/>).</summary>
    TypeClassification = 1
}

// ─────────────────────────────────────────────────────────────────────────────
// Phone / Bank Report
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Indicates the type of contact number or account identifier being reported.
/// </summary>
public enum NumberType
{
    /// <summary>A mobile or landline phone number.</summary>
    Phone = 0,

    /// <summary>A bank account number.</summary>
    BankAccount = 1
}

/// <summary>
/// Represents the moderation/verification state of a phone or bank-account report.
/// </summary>
public enum VerificationStatus
{
    /// <summary>No moderator review has been performed yet.</summary>
    Unverified = 0,

    /// <summary>A moderator has confirmed the report is legitimate.</summary>
    Verified = 1,

    /// <summary>A moderator has determined the report is inaccurate or malicious.</summary>
    FalseReport = 2
}

// ─────────────────────────────────────────────────────────────────────────────
// Emergency Contact
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Classifies the type of emergency contact or hotline.
/// </summary>
public enum ContactType
{
    /// <summary>Police / law-enforcement (e.g., 113).</summary>
    Police = 0,

    /// <summary>Fire department / civil defence (e.g., 114).</summary>
    FireFighter = 1,

    /// <summary>Ambulance / medical emergency (e.g., 115).</summary>
    Ambulance = 2,

    /// <summary>Dedicated anti-scam or cybercrime hotline.</summary>
    AntiScam = 3,

    /// <summary>Any other type of emergency or support contact.</summary>
    Other = 99
}

// ─────────────────────────────────────────────────────────────────────────────
// Travel Route
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Lifecycle status of a user's travel route.
/// </summary>
public enum RouteStatus
{
    /// <summary>Route has been created but not yet started.</summary>
    Planned = 0,

    /// <summary>Route is currently being tracked in real-time.</summary>
    Active = 1,

    /// <summary>Tracking is temporarily paused by the user.</summary>
    Paused = 2,

    /// <summary>Route was completed successfully.</summary>
    Completed = 3,

    /// <summary>Route was abandoned before completion.</summary>
    Cancelled = 4
}

// ─────────────────────────────────────────────────────────────────────────────
// Route Alert
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Type of safety alert triggered during route tracking.
/// </summary>
public enum AlertType
{
    /// <summary>User's GPS position deviated beyond the acceptable threshold.</summary>
    Deviation = 0,

    /// <summary>User manually triggered an SOS distress signal.</summary>
    SOS = 1,

    /// <summary>User did not respond to a check-in prompt within the countdown period.</summary>
    NoResponse = 2
}

/// <summary>
/// Describes how a route alert was ultimately resolved.
/// </summary>
public enum AlertResolution
{
    /// <summary>User actively confirmed they are safe.</summary>
    UserConfirmedSafe = 0,

    /// <summary>User acknowledged and updated their planned route.</summary>
    UserChangedRoute = 1,

    /// <summary>System notified registered relative contacts about the alert.</summary>
    NotifiedRelatives = 2,

    /// <summary>Alert countdown reached zero with no action taken.</summary>
    Expired = 3
}

// ─────────────────────────────────────────────────────────────────────────────
// Risk Score
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Human-readable risk level derived from a computed risk score.
/// </summary>
public enum RiskLevel
{
    /// <summary>Score 0–24: minimal concern.</summary>
    Low = 0,

    /// <summary>Score 25–49: warrants monitoring.</summary>
    Medium = 1,

    /// <summary>Score 50–74: high likelihood of fraudulent activity.</summary>
    High = 2,

    /// <summary>Score 75–100: immediate action recommended.</summary>
    Critical = 3
}
