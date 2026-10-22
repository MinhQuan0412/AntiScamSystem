using AntiScam.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AntiScam.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    /// <summary>
    /// F12: Thống kê số lượng và xu hướng các loại lừa đảo theo thời gian.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetStatistics([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var fromDate = from ?? DateTime.UtcNow.AddMonths(-1);
        var toDate = to ?? DateTime.UtcNow;

        var result = await _statisticsService.GetScamStatisticsAsync(fromDate, toDate);
        return Ok(result);
    }

    /// <summary>
    /// F12: Xuất báo cáo thống kê định dạng Excel (ClosedXML).
    /// </summary>
    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportExcel([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var fromDate = from ?? DateTime.UtcNow.AddMonths(-1);
        var toDate = to ?? DateTime.UtcNow;

        var result = await _statisticsService.ExportToExcelAsync(fromDate, toDate);
        if (!result.IsSuccess) return BadRequest(result);

        return File(result.Value!, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"BaoCaoLuaDao_{DateTime.UtcNow:yyyyMMdd}.xlsx");
    }

    /// <summary>
    /// F12: Xuất báo cáo thống kê định dạng PDF (QuestPDF).
    /// </summary>
    [HttpGet("export/pdf")]
    public async Task<IActionResult> ExportPdf([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var fromDate = from ?? DateTime.UtcNow.AddMonths(-1);
        var toDate = to ?? DateTime.UtcNow;

        var result = await _statisticsService.ExportToPdfAsync(fromDate, toDate);
        if (!result.IsSuccess) return BadRequest(result);

        return File(result.Value!, "application/pdf", $"BaoCaoLuaDao_{DateTime.UtcNow:yyyyMMdd}.pdf");
    }

    /// <summary>
    /// F04, F12: Thống kê hiệu năng các phiên bản mô hình ML.
    /// </summary>
    [HttpGet("model-performance")]
    public async Task<IActionResult> GetModelPerformance()
    {
        var result = await _statisticsService.GetModelPerformanceAsync();
        return Ok(result);
    }
}
