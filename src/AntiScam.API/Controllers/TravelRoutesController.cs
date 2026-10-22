using System.Security.Claims;
using AntiScam.Application.DTOs.Alert;
using AntiScam.Application.DTOs.TravelRoute;
using AntiScam.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AntiScam.API.Controllers;

[Authorize]
[ApiController]
[Route("api/travel-routes")]
public class TravelRoutesController : ControllerBase
{
    private readonly ITravelRouteService _routeService;

    public TravelRoutesController(ITravelRouteService routeService)
    {
        _routeService = routeService;
    }

    /// <summary>
    /// F10: Lập lộ trình du lịch mới.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateRoute([FromBody] CreateTravelRouteRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _routeService.CreateRouteAsync(request, userId);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveRoute()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _routeService.GetActiveRouteAsync(userId);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyRoutes()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _routeService.GetUserRoutesAsync(userId);
        return Ok(result);
    }

    [HttpPost("{id:guid}/start")]
    public async Task<IActionResult> StartTracking(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _routeService.StartTrackingAsync(id, userId);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id:guid}/stop")]
    public async Task<IActionResult> StopTracking(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _routeService.StopTrackingAsync(id, userId);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// F10: Báo cáo toạ độ GPS định kỳ từ trình duyệt người dùng.
    /// Kiểm tra khoảng cách đến lộ trình và phát hiện lệch tuyến.
    /// </summary>
    [HttpPost("report-location")]
    public async Task<IActionResult> ReportLocation([FromBody] ReportLocationRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _routeService.ReportLocationAsync(request, userId);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// F10: Phản hồi khi có cảnh báo lệch lộ trình ("Tôi ổn, tắt cảnh báo" hoặc "Đổi lộ trình").
    /// </summary>
    [HttpPost("alerts/respond")]
    public async Task<IActionResult> RespondToAlert([FromBody] AlertResponseRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _routeService.RespondToAlertAsync(request, userId);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// F11: Nút SOS khẩn cấp - gửi ngay toạ độ và thông báo đến người thân qua Email/Telegram.
    /// </summary>
    [HttpPost("{id:guid}/sos")]
    public async Task<IActionResult> TriggerSos(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _routeService.TriggerSosAsync(id, userId);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(new { Message = "Đã phát tín hiệu SOS khẩn cấp đến người thân của bạn!" });
    }
}
