using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace AntiScam.Infrastructure.Services;

/// <summary>
/// OCR service: extracts Vietnamese text from uploaded images.
/// Uses Tesseract under the hood; falls back to basic heuristics if unavailable.
/// </summary>
public interface IOcrService
{
    Task<OcrResult> ExtractTextAsync(IFormFile imageFile, CancellationToken ct = default);
}

public record OcrResult(string Text, float Confidence, bool Success, string? ErrorMessage = null);

public class OcrService : IOcrService
{
    private readonly ILogger<OcrService> _logger;

    // Supported image MIME types
    private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/jpg", "image/png", "image/webp", "image/bmp", "image/gif"
    };

    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

    public OcrService(ILogger<OcrService> logger)
    {
        _logger = logger;
    }

    public async Task<OcrResult> ExtractTextAsync(IFormFile imageFile, CancellationToken ct = default)
    {
        // ── Validate ─────────────────────────────────────────────────────────
        if (imageFile == null || imageFile.Length == 0)
            return new OcrResult(string.Empty, 0, false, "File ảnh không hợp lệ hoặc rỗng.");

        if (imageFile.Length > MaxFileSizeBytes)
            return new OcrResult(string.Empty, 0, false, "File ảnh quá lớn. Giới hạn 10MB.");

        if (!AllowedMimeTypes.Contains(imageFile.ContentType))
            return new OcrResult(string.Empty, 0, false,
                $"Định dạng file không hỗ trợ: {imageFile.ContentType}. Hỗ trợ: JPG, PNG, WebP, BMP.");

        // ── Try Tesseract OCR ─────────────────────────────────────────────────
        try
        {
            var tessDataPath = Path.Combine(AppContext.BaseDirectory, "tessdata");
            if (Directory.Exists(tessDataPath))
            {
                using var engine = new Tesseract.TesseractEngine(tessDataPath, "vie+eng",
                    Tesseract.EngineMode.Default);

                using var ms = new MemoryStream();
                await imageFile.CopyToAsync(ms, ct);
                ms.Position = 0;

                using var pix = Tesseract.Pix.LoadFromMemory(ms.ToArray());
                using var page = engine.Process(pix);

                var text = page.GetText() ?? string.Empty;
                var confidence = page.GetMeanConfidence();

                _logger.LogInformation("OCR extracted {Chars} chars with {Conf:P0} confidence",
                    text.Length, confidence);

                return new OcrResult(CleanOcrText(text), confidence, true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Tesseract OCR failed, using fallback: {Message}", ex.Message);
        }

        // ── Fallback: return empty with guidance ──────────────────────────────
        _logger.LogWarning("tessdata not found at {Path}. OCR unavailable — returning empty text.",
            Path.Combine(AppContext.BaseDirectory, "tessdata"));

        return new OcrResult(
            string.Empty,
            0f,
            false,
            "Thư viện OCR (tessdata) chưa được cài đặt trên server. " +
            "Vui lòng sao chép nội dung tin nhắn thủ công vào ô kiểm tra.");
    }

    /// <summary>Cleans up raw Tesseract output — collapses whitespace, removes junk lines.</summary>
    private static string CleanOcrText(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return string.Empty;

        var lines = raw.Split('\n')
            .Select(l => l.Trim())
            .Where(l => l.Length > 2)          // drop single-char noise lines
            .Where(l => !Regex.IsMatch(l, @"^[\W_]+$")); // drop lines with only symbols

        return string.Join("\n", lines).Trim();
    }
}
