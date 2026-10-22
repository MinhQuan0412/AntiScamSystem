using System.ComponentModel.DataAnnotations;

namespace AntiScam.Application.DTOs.Classification;

/// <summary>
/// Request payload for classifying a message or text snippet for scam content.
/// </summary>
public sealed class ClassifyMessageRequest
{
    /// <summary>
    /// Gets or sets the text content to classify.
    /// Must not be empty and may not exceed 5 000 characters.
    /// </summary>
    [Required]
    [MaxLength(5000, ErrorMessage = "Content must not exceed 5 000 characters.")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the user consents to the submitted content being
    /// stored (after anonymization) as a training data sample.
    /// </summary>
    public bool ConsentToStore { get; set; }
}
