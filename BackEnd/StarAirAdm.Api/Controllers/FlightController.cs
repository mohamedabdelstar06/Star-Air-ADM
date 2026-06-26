namespace StarAirAdm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FlightController : ControllerBase
{
    private readonly ISender _sender;

    public FlightController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateFlight(CreateFlightTripDto dto)
    {
        try
        {
            var command = new CreateFlightCommand(dto);
            var result = await _sender.Send(command);
            return Ok(result);
        }
        catch (System.Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateFlight(int id, UpdateFlightTripDto dto)
    {
        try
        {
            var command = new UpdateFlightCommand(id, dto);
            var result = await _sender.Send(command);
            if (result == null) return NotFound(new { message = "Flight not found." });
            return Ok(result);
        }
        catch (System.Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyFlights()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var query = new GetMyFlightsQuery(userId);
        var result = await _sender.Send(query);
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllFlightsQuery();
        var result = await _sender.Send(query);
        return Ok(result);
    }

    [HttpPatch("{id}/link")]
    public async Task<IActionResult> LinkAssessments(int id, LinkAssessmentDto dto)
    {
        var command = new LinkAssessmentsCommand(id, dto);
        var result = await _sender.Send(command);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPatch("{id}/complete")]
    public async Task<IActionResult> Complete(int id)
    {
        var command = new CompleteFlightCommand(id);
        var result = await _sender.Send(command);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new DeleteFlightCommand(id);
        var result = await _sender.Send(command);
        if (!result) return NotFound();
        return NoContent();
    }
}
