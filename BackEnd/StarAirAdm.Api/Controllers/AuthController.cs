namespace StarAirAdm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ISender sender, ILogger<AuthController> logger)
    {
        _sender = sender;
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

        var query = new CheckEmailQuery(request.Email);
        var response = await _sender.Send(query);
        
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

        var command = new LoginCommand(request.Email, request.Password);
        var response = await _sender.Send(command);
        
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

        var command = new RefreshTokenCommand(request.AccessToken, request.RefreshToken);
        var response = await _sender.Send(command);
        
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

        var command = new SetPasswordCommand(request.Email, request.InvitationToken, request.NewPassword);
        var result = await _sender.Send(command);
        
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

        var command = new ForgotPasswordCommand(request.Email);
        var result = await _sender.Send(command);
        
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

        var command = new ResetPasswordCommand(request.Email, request.Token, request.NewPassword);
        var result = await _sender.Send(command);
        
        if (!result)
        {
            _logger.LogWarning("400 Bad Request: Reset-Password failed for {Email}. Invalid reset token, or email mismatch.", request.Email);
            return BadRequest(new { message = "Invalid reset token or email" });
        }

        _logger.LogInformation("Successfully reset password for user {Email}", request.Email);
        return Ok(new { message = "Password has been reset successfully" });
    }
}
