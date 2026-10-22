using System.Security.Claims;
using AntiScam.Application.DTOs.PhoneReport;
using AntiScam.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AntiScam.API.Controllers;

[ApiController]
[Route("api/phone-reports")]
public class PhoneReportsController : ControllerBase
{
    private readonly IPhoneReportService _phoneReportService;

    public PhoneReportsController(IPhoneReportService phoneReportService)
    {
        _phoneReportService = phoneReportService;
    }

    /// <summary>
    /// F06: Tra cứu số điện thoại hoặc số tài khoản ngân hàng (Công khai, không cần đăng nhập).
    /// </summary>
    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup([FromQuery] string number)
    {
        if (string.IsNullOrWhiteSpace(number))
        {
            return BadRequest(new { Message = "Số tra cứu không được để trống." });
        }

        var result = await _phoneReportService.LookupAsync(number);
        return Ok(result);
    }

    /// <summary>
    /// F06: Báo cáo số điện thoại/số tài khoản nghi lừa đảo (Cần đăng nhập).
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Report([FromBody] CreatePhoneReportRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _phoneReportService.ReportAsync(request, userId);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// F08: Lấy danh sách báo cáo chờ kiểm duyệt (Quản trị viên).
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("pending-moderation")]
    public async Task<IActionResult> GetPendingModeration([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _phoneReportService.GetPendingModerationAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// F08: Kiểm duyệt báo cáo (Duyệt/Bác bỏ) (Quản trị viên).
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPost("moderate/{detailId:guid}")]
    public async Task<IActionResult> Moderate(Guid detailId, [FromBody] ModerateReportRequest request)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _phoneReportService.ModerateAsync(detailId, request.Approve, request.Note, adminId);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}

public record ModerateReportRequest(bool Approve, string Note);
