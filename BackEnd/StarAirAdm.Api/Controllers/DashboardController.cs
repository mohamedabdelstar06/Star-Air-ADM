namespace StarAirAdm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase
{
    private readonly ISender _sender;
    
    public DashboardController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var query = new GetDashboardStatsQuery();
            var stats = await _sender.Send(query);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Dashboard fail: {ex.Message} {(ex.InnerException?.Message)}" });
        }
    }
}
