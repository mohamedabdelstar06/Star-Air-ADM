using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using StarAirAdm.Application.DTOs.Auth;
using StarAirAdm.Application.Interfaces;
using StarAirAdm.Domain.Entities;
using StarAirAdm.Domain.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StarAirAdm.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration, IEmailService emailService, ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _configuration = configuration;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<CheckEmailResponseDto> CheckEmailStatusAsync(CheckEmailDto request)
    {
        try
        {
            _logger.LogInformation("Database lookup: Finding user by email {Email}", request.Email);
            var user = await _userManager.FindByEmailAsync(request.Email);
            
            if (user == null)
            {
                _logger.LogWarning("Email {Email} not found in the database.", request.Email);
                return new CheckEmailResponseDto { Exists = false };
            }

            _logger.LogInformation("Checking password status for {Email}", request.Email);
            var hasPassword = await _userManager.HasPasswordAsync(user);

            return new CheckEmailResponseDto
            {
                Exists = true,
                HasPassword = hasPassword,
                Status = user.Status.ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CRITICAL 500: Database or EF Core mapping error occurred in CheckEmailStatusAsync for email {Email}. Check migrations!", request.Email);
            throw; 
            
            // Let GlobalExceptionMiddleware catch and wrap this
        }
    }

    public async Task<TokenResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            return null; // Invalid credentials

        if (user.Status != UserStatus.Active)
            return null; // Ensure user is active

        var roles = await _userManager.GetRolesAsync(user);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secret = jwtSettings.GetValue<string>("Secret")!;
        var key = Encoding.ASCII.GetBytes(secret);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.FullName)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(jwtSettings.GetValue<int>("AccessTokenExpirationMinutes")),
            Issuer = jwtSettings.GetValue<string>("Issuer"),
            Audience = jwtSettings.GetValue<string>("Audience"),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var accessToken = tokenHandler.CreateToken(tokenDescriptor);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(jwtSettings.GetValue<int>("RefreshTokenExpirationDays"));
        await _userManager.UpdateAsync(user);

        return new TokenResponseDto
        {
            AccessToken = tokenHandler.WriteToken(accessToken),
            RefreshToken = refreshToken
        };
    }

    public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenDto model)
    {
        var principal = GetPrincipalFromExpiredToken(model.AccessToken);
        if (principal == null) return null;

        var email = principal.FindFirstValue(ClaimTypes.Email);
        if (email == null) return null;

        var user = await _userManager.FindByEmailAsync(email);
        
        if (user == null || user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiry <= DateTime.UtcNow)
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secret = jwtSettings.GetValue<string>("Secret")!;
        var key = Encoding.ASCII.GetBytes(secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(principal.Claims),
            Expires = DateTime.UtcNow.AddMinutes(jwtSettings.GetValue<int>("AccessTokenExpirationMinutes")),
            Issuer = jwtSettings.GetValue<string>("Issuer"),
            Audience = jwtSettings.GetValue<string>("Audience"),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var newAccessToken = tokenHandler.CreateToken(tokenDescriptor);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(jwtSettings.GetValue<int>("RefreshTokenExpirationDays"));
        await _userManager.UpdateAsync(user);

        return new TokenResponseDto
        {
            AccessToken = tokenHandler.WriteToken(newAccessToken),
            RefreshToken = newRefreshToken
        };
    }

    public async Task<bool> SetPasswordAsync(SetPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return false;

        // Verify the exact invitation token
        if (user.InvitationToken != model.InvitationToken) return false;
        
        // Ensure the token has not expired
        if (user.InvitationTokenExpiry < DateTime.UtcNow) return false;

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);

        if (result.Succeeded)
        {
            user.Status = UserStatus.Active;
            user.InvitationToken = null;
            user.InvitationTokenExpiry = null;
            await _userManager.UpdateAsync(user);
            return true;
        }

        return false;
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || user.Status != UserStatus.Active) return false;

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        
        try
        {
            await _emailService.SendPasswordResetEmailAsync(user.Email, user.FullName, resetToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return false;

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
        return result.Succeeded;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secret = jwtSettings.GetValue<string>("Secret")!;
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = jwtSettings.GetValue<string>("Audience"),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
            ValidateLifetime = false // Here we are saying that we don't care about the token's expiration date
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}
