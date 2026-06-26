namespace StarAirAdm.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AuditLogController : ControllerBase
{
    private readonly ISender _sender;

    public AuditLogController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllLogs()
    {
        var query = new GetAllAuditLogsQuery();
        var logs = await _sender.Send(query);
        return Ok(logs);
    }
}
