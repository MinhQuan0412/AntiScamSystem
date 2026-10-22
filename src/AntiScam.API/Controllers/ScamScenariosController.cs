using System.Security.Claims;
using AntiScam.Application.DTOs.ScamScenario;
using AntiScam.Application.Services;
using AntiScam.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AntiScam.API.Controllers;

[ApiController]
[Route("api/scam-scenarios")]
public class ScamScenariosController : ControllerBase
{
    private readonly IScamScenarioService _scenarioService;

    public ScamScenariosController(IScamScenarioService scenarioService)
    {
        _scenarioService = scenarioService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] ScamType? type = null,
        [FromQuery] ScamStatus? status = null)
    {
        var result = await _scenarioService.GetAllAsync(page, pageSize, type, status);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _scenarioService.GetByIdAsync(id);
        if (!result.IsSuccess) return NotFound(result);
        return Ok(result);
    }

    [HttpGet("{id:guid}/variants")]
    public async Task<IActionResult> GetVariants(Guid id)
    {
        var result = await _scenarioService.GetVariantsAsync(id);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        var result = await _scenarioService.SearchAsync(q ?? string.Empty);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScamScenarioRequest request)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _scenarioService.CreateAsync(request, adminId);
        if (!result.IsSuccess) return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateScamScenarioRequest request)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _scenarioService.UpdateAsync(id, request, adminId);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _scenarioService.DeleteAsync(id, adminId);
        if (!result.IsSuccess) return BadRequest(result);
        return NoContent();
    }
}
