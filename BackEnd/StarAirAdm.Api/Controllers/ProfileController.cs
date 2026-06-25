using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarAirAdm.Application.DTOs.Users;
using StarAirAdm.Application.Interfaces;

namespace StarAirAdm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Any authenticated user can access their own profile
public class ProfileController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IWebHostEnvironment _env;

    public ProfileController(IUserService userService, IWebHostEnvironment env)
    {
        _userService = userService;
        _env = env;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null) return NotFound(new { message = "User profile not found" });

        return Ok(user);
    }

    [HttpPatch("me")]
    public async Task<IActionResult> PatchMe([FromForm] UpdateUserDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var user = await _userService.UpdateUserAsync(userId, request);
        if (user == null) return NotFound(new { message = "User profile not found or update failed" });

        return Ok(user);
    }

    [HttpPost("me/picture")]
    public async Task<IActionResult> UploadPicture(IFormFile file)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        if (file == null || file.Length == 0) return BadRequest(new { message = "Upload a valid file." });

        if (file.Length > 2 * 1024 * 1024) return BadRequest(new { message = "File too large. Max 2MB allowed." });

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension)) return BadRequest(new { message = "Invalid file type." });

        var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "profiles");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{userId}_{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        var fileUrl = $"/uploads/profiles/{uniqueFileName}";

        // Update User ProfileImageUrl field
        var existingUser = await _userService.GetUserByIdAsync(userId);
        if (existingUser == null) return NotFound(new { message = "User not found" });

        // Update the image using UpdateUserAsync
        var updateDto = new UpdateUserDto
        {
            FullName = existingUser.FullName,
            LicenseNumber = existingUser.LicenseNumber,
            MedicalClass = existingUser.MedicalClass,
            Rank = existingUser.Rank,
            TotalFlightHours = existingUser.TotalFlightHours,
            ProfileImageUrl = fileUrl
        };

        var updatedUser = await _userService.UpdateUserAsync(userId, updateDto);
        return Ok(updatedUser);
    }
}
