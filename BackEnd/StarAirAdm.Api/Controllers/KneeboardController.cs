using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarAirAdm.Application.DTOs.Kneeboard;
using StarAirAdm.Application.Interfaces;
using System.Security.Claims;

namespace StarAirAdm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Pilot")]
public class KneeboardController : ControllerBase
{
    private readonly IKneeboardService _svc;
    public KneeboardController(IKneeboardService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateKneeboardNoteDto dto)
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _svc.CreateAsync(dto, pilotId);
        return result == null ? BadRequest() : Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _svc.GetByPilotAsync(pilotId));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateKneeboardNoteDto dto)
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _svc.UpdateAsync(id, dto, pilotId);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var ok = await _svc.DeleteAsync(id, pilotId);
        return ok ? NoContent() : NotFound();
    }
}
