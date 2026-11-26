namespace AntiScam.Application.DTOs.Classification;

/// <summary>
/// Response after extracting text from an uploaded image via OCR.
/// </summary>
public sealed class ClassifyImageResponse
{
    /// <summary>Raw text extracted from the image by OCR.</summary>
    public string ExtractedText { get; set; } = string.Empty;

    /// <summary>OCR confidence score (0–100).</summary>
    public float OcrConfidence { get; set; }

    /// <summary>Full classification result based on the extracted text.</summary>
    public ClassifyMessageResponse? ClassificationResult { get; set; }

    /// <summary>True if OCR extracted meaningful text (> 10 characters).</summary>
    public bool HasMeaningfulText => ExtractedText.Trim().Length > 10;
}
