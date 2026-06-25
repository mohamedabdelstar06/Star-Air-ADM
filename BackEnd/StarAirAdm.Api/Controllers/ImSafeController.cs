using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarAirAdm.Application.DTOs.ImSafe;
using StarAirAdm.Application.Interfaces;
using System.Security.Claims;

namespace StarAirAdm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ImSafeController : ControllerBase
{
    private readonly IImSafeService _svc;
    public ImSafeController(IImSafeService svc) => _svc = svc;

   
    [HttpPost]
    [Authorize(Roles = "Pilot")]
    public async Task<IActionResult> Create([FromBody] CreateImSafeDto dto)
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _svc.CreateAsync(dto, pilotId);
        return result == null ? BadRequest() : CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
   [HttpGet("my")]
    [Authorize(Roles = "Pilot")]
    public async Task<IActionResult> GetMy()
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _svc.GetByPilotAsync(pilotId));
    }

   
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

    [HttpGet("pilot/{pilotId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetByPilot(string pilotId)
        => Ok(await _svc.GetByPilotAsync(pilotId));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _svc.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var ok = await _svc.DeleteAsync(id, userId);
        return ok ? NoContent() : NotFound();
    }
}
