namespace StarAirAdm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaveController : ControllerBase
{
    private readonly ISender _sender;
    
    public PaveController(ISender sender) => _sender = sender;

    [HttpPost]
    [Authorize(Roles = "Pilot")]
    public async Task<IActionResult> Create([FromBody] CreatePaveDto dto)
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var command = new CreatePaveCommand(dto, pilotId);
        var result = await _sender.Send(command);
        return result == null ? BadRequest() : CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("my")]
    [Authorize(Roles = "Pilot")]
    public async Task<IActionResult> GetMy()
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var query = new GetMyPaveQuery(pilotId);
        return Ok(await _sender.Send(query));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllPaveQuery();
        return Ok(await _sender.Send(query));
    }

    [HttpGet("pilot/{pilotId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetByPilot(string pilotId)
    {
        var query = new GetPaveByPilotQuery(pilotId);
        return Ok(await _sender.Send(query));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetPaveByIdQuery(id);
        var result = await _sender.Send(query);
        return result == null ? NotFound() : Ok(result);
    }
}
