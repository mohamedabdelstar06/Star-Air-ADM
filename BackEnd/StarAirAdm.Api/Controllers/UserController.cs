using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarAirAdm.Application.DTOs.Users;
using StarAirAdm.Application.Interfaces;

namespace StarAirAdm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request)
    {
        var (user, errorMessage) = await _userService.CreateUserAsync(request);
        if (user == null) return BadRequest(new { message = errorMessage ?? "User creation failed." });

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null) return NotFound(new { message = "User not found" });

        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto request)
    {
        var user = await _userService.UpdateUserAsync(id, request);
        if (user == null) return NotFound(new { message = "User not found" });

        return Ok(user);
    }

    [HttpPatch("{id}/toggle-status")]
    public async Task<IActionResult> ToggleUserStatus(string id)
    {
        var success = await _userService.ToggleUserStatusAsync(id);
        if (!success) return NotFound(new { message = "User not found or could not update status" });

        return Ok(new { message = "User status toggled successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var success = await _userService.DeleteUserAsync(id);
        if (!success) return NotFound(new { message = "User not found or could not be deleted" });

        return Ok(new { message = "User deleted successfully" });
    }
}
