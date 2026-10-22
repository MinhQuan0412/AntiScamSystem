using System.Security.Claims;
using AntiScam.Application.DTOs.Classification;
using AntiScam.Application.Services;
using AntiScam.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AntiScam.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassificationController : ControllerBase
{
    private readonly IClassificationService _classificationService;

    public ClassificationController(IClassificationService classificationService)
    {
        _classificationService = classificationService;
    }

    /// <summary>
    /// F05: Kiểm tra tin nhắn / tình huống nghi ngờ (phân loại, dấu hiệu, khuyến nghị).
    /// Người dùng dán nội dung vào để kiểm tra trực tiếp không cần đăng nhập.
    /// </summary>
    [HttpPost("classify")]
    public async Task<IActionResult> Classify([FromBody] ClassifyMessageRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _classificationService.ClassifyAsync(request, userId);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("models")]
    public async Task<IActionResult> GetModels()
    {
        var result = await _classificationService.GetModelVersionsAsync();
        return Ok(result);
    }

    /// <summary>
    /// F04, F07: Kích hoạt huấn luyện lại mô hình phân lớp.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPost("train")]
    public async Task<IActionResult> Train([FromBody] TriggerTrainApiRequest request)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _classificationService.TriggerRetrainingAsync(request.DataBatch, request.ModelType, adminId);
        return Ok(result);
    }

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
