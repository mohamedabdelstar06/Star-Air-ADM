using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarAirAdm.Application.DTOs.SmartWatch;
using StarAirAdm.Application.Interfaces;
using System.Security.Claims;

namespace StarAirAdm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SmartWatchController : ControllerBase
{
    private readonly ISmartWatchService _svc;
    public SmartWatchController(ISmartWatchService svc) => _svc = svc;

    [HttpPost("readings")]
    [Authorize(Roles = "Pilot")]
    public async Task<IActionResult> AddReading([FromBody] CreateSmartWatchReadingDto dto)
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _svc.AddReadingAsync(dto, pilotId);
        return result == null ? BadRequest() : Ok(result);
    }

    [HttpGet("readings")]
    [Authorize(Roles = "Pilot")]
    public async Task<IActionResult> GetReadings()
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _svc.GetByPilotAsync(pilotId));
    }

    [HttpGet("readings/{id}")]
    [Authorize(Roles = "Admin,Pilot")]
    public async Task<IActionResult> GetReadingById(int id)
    {
        var result = await _svc.GetByIdAsync(id);
        if (result == null) return NotFound();
        // optionally check if pilot matches or is admin
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");
        if (!isAdmin && result.PilotId != userId) return Forbid();
        return Ok(result);
    }

    [HttpGet("analysis")]
    [Authorize(Roles = "Pilot")]
    public async Task<IActionResult> GetAnalysis()
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _svc.GetAnalysisAsync(pilotId);
        return result == null
            ? NotFound(new { message = "No SmartWatch data found for the past 7 days." })
            : Ok(result);
    }
}
