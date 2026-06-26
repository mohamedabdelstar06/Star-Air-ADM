namespace StarAirAdm.Application.Features.Users.Commands;

public record ToggleUserStatusCommand(string Id) : IRequest<bool>;

public class ToggleUserStatusCommandHandler : IRequestHandler<ToggleUserStatusCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ToggleUserStatusCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user == null) return false;

        if (user.Status == UserStatus.Active)
        {
            user.Status = UserStatus.Inactive;
        }
        else if (user.Status == UserStatus.Inactive)
        {
            user.Status = UserStatus.Active;
        }

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }
}
