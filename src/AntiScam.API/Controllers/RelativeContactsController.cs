using System.Security.Claims;
using AntiScam.Application.DTOs.RelativeContact;
using AntiScam.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AntiScam.API.Controllers;

[Authorize]
[ApiController]
[Route("api/relative-contacts")]
public class RelativeContactsController : ControllerBase
{
    private readonly IRelativeContactService _contactService;

    public RelativeContactsController(IRelativeContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyRelatives()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _contactService.GetUserRelativesAsync(userId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRelativeContactRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _contactService.CreateAsync(request, userId);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateRelativeContactRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _contactService.UpdateAsync(id, request, userId);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _contactService.DeleteAsync(id, userId);
        if (!result.IsSuccess) return BadRequest(result);
        return NoContent();
    }
}
