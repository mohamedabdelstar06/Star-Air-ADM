namespace StarAirAdm.Application.Features.Auth.Queries;

public record CheckEmailQuery(string Email) : IRequest<CheckEmailResponseDto>;

public class CheckEmailQueryHandler : IRequestHandler<CheckEmailQuery, CheckEmailResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CheckEmailQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<CheckEmailResponseDto> Handle(CheckEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        
        if (user == null)
        {
            return new CheckEmailResponseDto { Exists = false };
        }

        var hasPassword = await _userManager.HasPasswordAsync(user);

        return new CheckEmailResponseDto
        {
            Exists = true,
            HasPassword = hasPassword,
            Status = user.Status.ToString()
        };
    }
}
