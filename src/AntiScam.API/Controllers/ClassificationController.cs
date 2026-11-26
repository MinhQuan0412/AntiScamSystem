using System.Security.Claims;
using AntiScam.Application.DTOs.Classification;
using AntiScam.Application.Services;
using AntiScam.Domain.Enums;
using AntiScam.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AntiScam.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassificationController : ControllerBase
{
    private readonly IClassificationService _classificationService;
    private readonly IChatAnalysisService   _chatAnalysisService;
    private readonly IOcrService            _ocrService;

    public ClassificationController(
        IClassificationService classificationService,
        IChatAnalysisService   chatAnalysisService,
        IOcrService            ocrService)
    {
        _classificationService = classificationService;
        _chatAnalysisService   = chatAnalysisService;
        _ocrService            = ocrService;
    }

    // ── F05: Kiểm tra văn bản ────────────────────────────────────────────────

    /// <summary>
    /// F05: Kiểm tra tin nhắn / tình huống nghi ngờ bằng văn bản.
    /// Người dùng dán nội dung vào — không cần đăng nhập.
    /// </summary>
    [HttpPost("classify")]
    public async Task<IActionResult> Classify([FromBody] ClassifyMessageRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _classificationService.ClassifyAsync(request, userId);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    // ── F05 (mở rộng): Kiểm tra từ ảnh chụp ────────────────────────────────

    /// <summary>
    /// F05 (mở rộng): Upload ảnh chụp màn hình tin nhắn →
    /// OCR trích xuất text → phân tích lừa đảo tự động.
    /// Content-Type: multipart/form-data, field name: "image"
    /// </summary>
    [HttpPost("classify-image")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
    public async Task<IActionResult> ClassifyImage(IFormFile image, [FromForm] bool consentToStore = false)
    {
        if (image == null || image.Length == 0)
            return BadRequest(new { Message = "Vui lòng chọn file ảnh để kiểm tra." });

        // 1. OCR extract text
        var ocrResult = await _ocrService.ExtractTextAsync(image);
        if (!ocrResult.Success || string.IsNullOrWhiteSpace(ocrResult.Text))
        {
            return Ok(new ClassifyImageResponse
            {
                ExtractedText         = string.Empty,
                OcrConfidence         = 0,
                ClassificationResult  = null
            });
        }

        // 2. Classify extracted text
        var userId      = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var classifyReq = new ClassifyMessageRequest
        {
            Content        = ocrResult.Text,
            ConsentToStore = consentToStore
        };

        var classifyResult = await _classificationService.ClassifyAsync(classifyReq, userId);

        return Ok(new ClassifyImageResponse
        {
            ExtractedText        = ocrResult.Text,
            OcrConfidence        = ocrResult.Confidence,
            ClassificationResult = classifyResult.IsSuccess ? classifyResult.Value : null
        });
    }

    // ── F05 (mở rộng): Phân tích đoạn chat ──────────────────────────────────

    /// <summary>
    /// F05 (mở rộng): Phân tích toàn bộ đoạn chat —
    /// phát hiện thủ đoạn lừa đảo theo ngữ cảnh trò chuyện.
    /// Gửi mảng tin nhắn với sender = "them" / "me".
    /// </summary>
    [HttpPost("classify-chat")]
    public async Task<IActionResult> ClassifyChat([FromBody] ClassifyChatRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _chatAnalysisService.AnalyzeChatAsync(request, userId);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    // ── Admin: Quản lý model ─────────────────────────────────────────────────

    /// <summary>F04: Danh sách phiên bản mô hình và chỉ số đánh giá.</summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("models")]
    public async Task<IActionResult> GetModels()
    {
        var result = await _classificationService.GetModelVersionsAsync();
        return Ok(result);
    }

    /// <summary>F04, F07: Kích hoạt huấn luyện lại mô hình.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost("train")]
    public async Task<IActionResult> Train([FromBody] TriggerTrainApiRequest request)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result  = await _classificationService.TriggerRetrainingAsync(
            request.DataBatch, request.ModelType, adminId);
        return Ok(result);
    }

    /// <summary>F04: Kiểm tra trạng thái tiến trình huấn luyện.</summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("train/{jobId}/status")]
    public async Task<IActionResult> GetTrainingStatus(string jobId)
    {
        var result = await _classificationService.GetTrainingStatusAsync(jobId);
        if (!result.IsSuccess) return NotFound(result);
        return Ok(result);
    }
}

public record TriggerTrainApiRequest(string DataBatch, ModelType ModelType);
