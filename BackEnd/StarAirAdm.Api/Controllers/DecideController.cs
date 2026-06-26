namespace StarAirAdm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DecideController : ControllerBase
{
    private readonly ISender _sender;
    
    public DecideController(ISender sender) => _sender = sender;

    [HttpPost("sessions")]
    [Authorize(Roles = "Pilot")]
    public async Task<IActionResult> CreateSession([FromBody] CreateDecideSessionDto dto)
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var command = new CreateDecideSessionCommand(dto, pilotId);
        var result = await _sender.Send(command);
        return result == null ? BadRequest() : CreatedAtAction(nameof(GetSession), new { id = result.Id }, result);
    }

    [HttpGet("sessions/my")]
    [Authorize(Roles = "Pilot")]
    public async Task<IActionResult> GetMySessions()
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var query = new GetMyDecideSessionsQuery(pilotId);
        return Ok(await _sender.Send(query));
    }

    [HttpGet("sessions")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllSessions()
    {
        var query = new GetAllDecideSessionsQuery();
        return Ok(await _sender.Send(query));
    }

    [HttpGet("sessions/{id:int}")]
    public async Task<IActionResult> GetSession(int id)
    {
        var query = new GetDecideSessionByIdQuery(id);
        var result = await _sender.Send(query);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost("sessions/{sessionId:int}/steps")]
    [Authorize(Roles = "Pilot")]
    public async Task<IActionResult> AddStep(int sessionId, [FromBody] CreateDecideStepDto dto)
    {
        var command = new AddDecideStepCommand(sessionId, dto);
        var result = await _sender.Send(command);
        return result == null
            ? BadRequest(new { message = "Session not found or already completed." })
            : Ok(result);
    }

    [HttpPatch("sessions/{sessionId:int}/complete")]
    [Authorize(Roles = "Pilot")]
    public async Task<IActionResult> CompleteSession(int sessionId)
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var command = new CompleteDecideSessionCommand(sessionId, pilotId);
        var ok = await _sender.Send(command);
        return ok ? Ok(new { message = "Session completed." }) : NotFound();
    }
}
