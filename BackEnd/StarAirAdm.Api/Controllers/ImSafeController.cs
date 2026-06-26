namespace StarAirAdm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ImSafeController : ControllerBase
{
    private readonly ISender _sender;
    
    public ImSafeController(ISender sender) => _sender = sender;

    [HttpPost]
    [Authorize(Roles = "Pilot")]
    public async Task<IActionResult> Create([FromBody] CreateImSafeDto dto)
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var command = new CreateImSafeCommand(dto, pilotId);
        var result = await _sender.Send(command);
        return result == null ? BadRequest() : CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("my")]
    [Authorize(Roles = "Pilot")]
    public async Task<IActionResult> GetMy()
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var query = new GetMyImSafeQuery(pilotId);
        return Ok(await _sender.Send(query));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllImSafeQuery();
        return Ok(await _sender.Send(query));
    }

    [HttpGet("pilot/{pilotId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetByPilot(string pilotId)
    {
        var query = new GetImSafeByPilotQuery(pilotId);
        return Ok(await _sender.Send(query));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetImSafeByIdQuery(id);
        var result = await _sender.Send(query);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var command = new DeleteImSafeCommand(id, userId);
        var ok = await _sender.Send(command);
        return ok ? NoContent() : NotFound();
    }
}
