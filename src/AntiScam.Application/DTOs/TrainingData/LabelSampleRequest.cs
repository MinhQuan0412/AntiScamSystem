using System.ComponentModel.DataAnnotations;
using AntiScam.Domain.Enums;

namespace AntiScam.Application.DTOs.TrainingData;

/// <summary>
/// Request payload for labelling (annotating) an existing training data sample.
/// </summary>
public sealed class LabelSampleRequest
{
    /// <summary>Gets or sets whether the sample is confirmed as a scam.</summary>
    public bool IsScam { get; set; }

    /// <summary>Gets or sets the scam type label assigned by the reviewer.</summary>
    public ScamType? ScamType { get; set; }

    /// <summary>Gets or sets any additional notes from the admin reviewer.</summary>
    [MaxLength(1000)]
    public string? LabelNotes { get; set; }
}
