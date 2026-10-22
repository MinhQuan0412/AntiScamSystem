using AntiScam.Application.DTOs.EmergencyContact;
using AntiScam.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AntiScam.API.Controllers;

[ApiController]
[Route("api/emergency-contacts")]
public class EmergencyContactsController : ControllerBase
{
    private readonly IEmergencyContactService _emergencyService;

    public EmergencyContactsController(IEmergencyContactService emergencyService)
    {
        _emergencyService = emergencyService;
    }

    /// <summary>
    /// F09: Danh bạ số khẩn cấp theo địa phương (Công khai).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetByProvince([FromQuery] string? provinceCode)
    {
        var result = await _emergencyService.GetByProvinceAsync(provinceCode);
        return Ok(result);
    }

    /// <summary>
    /// F09: Danh bạ số khẩn cấp toàn quốc (113, 114, 115).
    /// </summary>
    [HttpGet("national")]
    public async Task<IActionResult> GetNational()
    {
        var result = await _emergencyService.GetNationalAsync();
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmergencyContactRequest request)
    {
        var result = await _emergencyService.CreateAsync(request);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateEmergencyContactRequest request)
    {
        var result = await _emergencyService.UpdateAsync(id, request);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _emergencyService.DeleteAsync(id);
        if (!result.IsSuccess) return BadRequest(result);
        return NoContent();
    }
}
