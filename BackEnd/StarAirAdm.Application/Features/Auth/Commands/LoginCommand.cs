namespace StarAirAdm.Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<TokenResponseDto?>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenResponseDto?>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(UserManager<ApplicationUser> userManager, IJwtService jwtService, IConfiguration configuration)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    public async Task<TokenResponseDto?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return null;

        if (user.Status != UserStatus.Active)
            return null;

        var (accessToken, refreshToken) = await _jwtService.GenerateTokensAsync(user);

        var jwtSettings = _configuration.GetSection("JwtSettings");
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(jwtSettings.GetValue<int>("RefreshTokenExpirationDays"));
        await _userManager.UpdateAsync(user);

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}
