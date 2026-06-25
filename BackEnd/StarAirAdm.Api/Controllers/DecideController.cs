using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarAirAdm.Application.DTOs.Decide;
using StarAirAdm.Application.Interfaces;
using System.Security.Claims;

namespace StarAirAdm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DecideController : ControllerBase
{
    private readonly IDecideService _svc;
    public DecideController(IDecideService svc) => _svc = svc;

    [HttpPost("sessions")]
    [Authorize(Roles = "Pilot")]
    public async Task<IActionResult> CreateSession([FromBody] CreateDecideSessionDto dto)
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _svc.CreateSessionAsync(dto, pilotId);
        return result == null ? BadRequest() : CreatedAtAction(nameof(GetSession), new { id = result.Id }, result);
    }

    [HttpGet("sessions/my")]
    [Authorize(Roles = "Pilot")]
    public async Task<IActionResult> GetMySessions()
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _svc.GetByPilotAsync(pilotId));
    }

    [HttpGet("sessions")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllSessions() => Ok(await _svc.GetAllAsync());

    [HttpGet("sessions/{id:int}")]
    public async Task<IActionResult> GetSession(int id)
    {
        var result = await _svc.GetSessionByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost("sessions/{sessionId:int}/steps")]
    [Authorize(Roles = "Pilot")]
    public async Task<IActionResult> AddStep(int sessionId, [FromBody] CreateDecideStepDto dto)
    {
        var result = await _svc.AddStepAsync(sessionId, dto);
        return result == null
            ? BadRequest(new { message = "Session not found or already completed." })
            : Ok(result);
    }

    [HttpPatch("sessions/{sessionId:int}/complete")]
    [Authorize(Roles = "Pilot")]
    public async Task<IActionResult> CompleteSession(int sessionId)
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var ok = await _svc.CompleteSessionAsync(sessionId, pilotId);
        return ok ? Ok(new { message = "Session completed." }) : NotFound();
    }
}
