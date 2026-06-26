namespace StarAirAdm.Application.Features.Auth.Commands;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<TokenResponseDto?>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResponseDto?>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;

    public RefreshTokenCommandHandler(UserManager<ApplicationUser> userManager, IJwtService jwtService, IConfiguration configuration)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    public async Task<TokenResponseDto?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null) return null;

        var email = principal.FindFirstValue(ClaimTypes.Email);
        if (email == null) return null;

        var user = await _userManager.FindByEmailAsync(email);
        
        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiry <= DateTime.UtcNow)
            return null;

        var (newAccessToken, newRefreshToken) = await _jwtService.GenerateTokensAsync(user);

        var jwtSettings = _configuration.GetSection("JwtSettings");
        
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(jwtSettings.GetValue<int>("RefreshTokenExpirationDays"));
        await _userManager.UpdateAsync(user);

        return new TokenResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
}
