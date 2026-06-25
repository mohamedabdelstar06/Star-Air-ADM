using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarAirAdm.Application.Interfaces;

namespace StarAirAdm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _svc;
    public DashboardController(IDashboardService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            return Ok(await _svc.GetStatsAsync());
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Dashboard fail: {ex.Message} {(ex.InnerException?.Message)}" });
        }
    }
}
