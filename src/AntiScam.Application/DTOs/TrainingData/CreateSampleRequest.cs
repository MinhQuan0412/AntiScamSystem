using System.ComponentModel.DataAnnotations;
using AntiScam.Domain.Enums;

namespace AntiScam.Application.DTOs.TrainingData;

/// <summary>
/// Request payload for submitting a new training data sample.
/// </summary>
public sealed class CreateSampleRequest
{
    /// <summary>Gets or sets the text content to submit as a training sample.</summary>
    [Required]
    [MaxLength(5000)]
    public string Content { get; set; } = string.Empty;

    /// <summary>Gets or sets the classification task this sample will contribute to.</summary>
    [Required]
    public ClassificationTask ClassificationTask { get; set; }

    /// <summary>Gets or sets whether the submitter believes this content is a scam.</summary>
    public bool IsScam { get; set; }

    /// <summary>Gets or sets the scam type if known at submission time.</summary>
    public ScamType? ScamType { get; set; }

    /// <summary>Gets or sets whether the user consents to storing and using this content for training.</summary>
    [Required]
    public bool ConsentToStore { get; set; }

    public string? DataBatch { get; set; }
    public string? Source { get; set; }
}
