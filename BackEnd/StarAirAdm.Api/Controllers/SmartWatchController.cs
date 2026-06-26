namespace StarAirAdm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SmartWatchController : ControllerBase
{
    private readonly ISender _sender;
    
    public SmartWatchController(ISender sender) => _sender = sender;

    [HttpPost("readings")]
    [Authorize(Roles = "Pilot")]
    public async Task<IActionResult> AddReading([FromBody] CreateSmartWatchReadingDto dto)
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var command = new AddSmartWatchReadingCommand(dto, pilotId);
        var result = await _sender.Send(command);
        return result == null ? BadRequest() : Ok(result);
    }

    [HttpGet("readings")]
    [Authorize(Roles = "Pilot")]
    public async Task<IActionResult> GetReadings()
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var query = new GetMySmartWatchReadingsQuery(pilotId);
        return Ok(await _sender.Send(query));
    }

    [HttpGet("readings/{id}")]
    [Authorize(Roles = "Admin,Pilot")]
    public async Task<IActionResult> GetReadingById(int id)
    {
        var query = new GetSmartWatchReadingByIdQuery(id);
        var result = await _sender.Send(query);
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
        var query = new GetSmartWatchAnalysisQuery(pilotId);
        var result = await _sender.Send(query);
        return result == null
            ? NotFound(new { message = "No SmartWatch data found for the past 7 days." })
            : Ok(result);
    }
}
