namespace StarAirAdm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UserController : ControllerBase
{
    private readonly ISender _sender;

    public UserController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request)
    {
        var command = new CreateUserCommand(request);
        var (user, errorMessage) = await _sender.Send(command);
        if (user == null) return BadRequest(new { message = errorMessage ?? "User creation failed." });

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var query = new GetUsersQuery();
        var users = await _sender.Send(query);
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var query = new GetUserByIdQuery(id);
        var user = await _sender.Send(query);
        if (user == null) return NotFound(new { message = "User not found" });

        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto request)
    {
        var command = new UpdateUserCommand(id, request);
        var user = await _sender.Send(command);
        if (user == null) return NotFound(new { message = "User not found" });

        return Ok(user);
    }

    [HttpPatch("{id}/toggle-status")]
    public async Task<IActionResult> ToggleUserStatus(string id)
    {
        var command = new ToggleUserStatusCommand(id);
        var success = await _sender.Send(command);
        if (!success) return NotFound(new { message = "User not found or could not update status" });

        return Ok(new { message = "User status toggled successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var command = new DeleteUserCommand(id);
        var success = await _sender.Send(command);
        if (!success) return NotFound(new { message = "User not found or could not be deleted" });

        return Ok(new { message = "User deleted successfully" });
    }
}
