namespace StarAirAdm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Pilot")]
public class KneeboardController : ControllerBase
{
    private readonly ISender _sender;
    
    public KneeboardController(ISender sender) => _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateKneeboardNoteDto dto)
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var command = new CreateKneeboardNoteCommand(dto, pilotId);
        var result = await _sender.Send(command);
        return result == null ? BadRequest() : Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var query = new GetMyKneeboardNotesQuery(pilotId);
        return Ok(await _sender.Send(query));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateKneeboardNoteDto dto)
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var command = new UpdateKneeboardNoteCommand(id, dto, pilotId);
        var result = await _sender.Send(command);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var pilotId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var command = new DeleteKneeboardNoteCommand(id, pilotId);
        var ok = await _sender.Send(command);
        return ok ? NoContent() : NotFound();
    }
}
