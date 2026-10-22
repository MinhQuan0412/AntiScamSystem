using System.Security.Claims;
using AntiScam.Application.DTOs.TrainingData;
using AntiScam.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AntiScam.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/training-data")]
public class TrainingDataController : ControllerBase
{
    private readonly ITrainingDataService _trainingDataService;

    public TrainingDataController(ITrainingDataService trainingDataService)
    {
        _trainingDataService = trainingDataService;
    }

    /// <summary>
    /// F03: Lấy danh sách mẫu dữ liệu phân loại.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? batch = null)
    {
        var result = await _trainingDataService.GetAllAsync(page, pageSize, batch);
        return Ok(result);
    }

    /// <summary>
    /// F03: Thêm mẫu dữ liệu mới (đã ẩn danh thông tin).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddSample([FromBody] CreateSampleRequest request)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _trainingDataService.AddSampleAsync(request, adminId);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// F03: Gán nhãn hoặc điều chỉnh nhãn cho mẫu dữ liệu.
    /// </summary>
    [HttpPut("{id:guid}/label")]
    public async Task<IActionResult> LabelSample(Guid id, [FromBody] LabelSampleRequest request)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _trainingDataService.LabelSampleAsync(id, request, adminId);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Xóa mẫu dữ liệu.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteSample(Guid id)
    {
        var result = await _trainingDataService.DeleteSampleAsync(id);
        if (!result.IsSuccess) return BadRequest(result);
        return NoContent();
    }
}
