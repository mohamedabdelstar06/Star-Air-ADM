using Microsoft.AspNetCore.Mvc;
using StarAirAdm.Application.DTOs.Auth;
using StarAirAdm.Application.Interfaces;

namespace StarAirAdm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("check-email")]
    public async Task<IActionResult> CheckEmail([FromBody] CheckEmailDto request)
    {
        _logger.LogInformation("Attempting to check status for email: {Email}", request.Email);
        
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            _logger.LogWarning("400 Bad Request: Check-Email called with empty or null Email payload.");
            return BadRequest(new { message = "Email payload is missing." });
        }

        var response = await _authService.CheckEmailStatusAsync(request);
        if (!response.Exists)
        {
            _logger.LogWarning("404 Not Found: Email {Email} does not currently exist in the database.", request.Email);
            return NotFound(new { message = "Email does not exist in the system" });
        }

        _logger.LogInformation("Successfully checked email status for {Email}. User exists.", request.Email);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        _logger.LogInformation("Processing login attempt for {Email}", request.Email);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("400 Bad Request: Login payload model is invalid for {Email}", request.Email);
            return BadRequest(ModelState);
        }

        var response = await _authService.LoginAsync(request);
        if (response == null)
        {
            _logger.LogWarning("401 Unauthorized: Login failed for {Email}. Invalid email or correct password mismatch.", request.Email);
            return Unauthorized(new { message = "Invalid email or password" });
        }

        _logger.LogInformation("Login successful. Issued JWT tokens for {Email}.", request.Email);
        return Ok(response);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto request)
    {
        _logger.LogInformation("Processing refresh token request.");

        var response = await _authService.RefreshTokenAsync(request);
        if (response == null)
        {
            _logger.LogWarning("401 Unauthorized: Failed to refresh token. Either the refresh token is expired, mismatched, or the user is invalid.");
            return Unauthorized(new { message = "Invalid or expired refresh token" });
        }

        _logger.LogInformation("Refresh token succeeded. New access token issued.");
        return Ok(response);
    }

    [HttpPost("set-password")]
    public async Task<IActionResult> SetPassword([FromBody] SetPasswordDto request)
    {
        _logger.LogInformation("Processing set-password request for newly invited user {Email}", request.Email);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("400 Bad Request: Set-Password payload model is invalid for {Email}", request.Email);
            return BadRequest(ModelState);
        }

        var result = await _authService.SetPasswordAsync(request);
        if (!result)
        {
            _logger.LogWarning("400 Bad Request: Set-Password failed for {Email}. Invitation token may be invalid, missing, or expired.", request.Email);
            return BadRequest(new { message = "Invalid or expired invitation token" });
        }

        _logger.LogInformation("Successfully set new password and activated account for {Email}.", request.Email);
        return Ok(new { message = "Password set successfully" });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
    {
        _logger.LogInformation("Processing forgot-password request for {Email}", request.Email);

        var result = await _authService.ForgotPasswordAsync(request.Email);
        if (!result)
        {
            _logger.LogWarning("Forgot Password notice: User {Email} does not exist, or is inactive. Responding with 200 OK regardless to prevent enumeration.", request.Email);
        }
        
        // Always return Ok to prevent email enumeration
        return Ok(new { message = "If the email is valid and active, a password reset link has been sent." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
    {
        _logger.LogInformation("Processing reset-password request for {Email}", request.Email);

        var result = await _authService.ResetPasswordAsync(request);
        if (!result)
        {
            _logger.LogWarning("400 Bad Request: Reset-Password failed for {Email}. Invalid reset token, or email mismatch.", request.Email);
            return BadRequest(new { message = "Invalid reset token or email" });
        }

        _logger.LogInformation("Successfully reset password for user {Email}", request.Email);
        return Ok(new { message = "Password has been reset successfully" });
    }
}
