namespace StarAirAdm.Application.Features.Auth.Commands;

public record SetPasswordCommand(string Email, string InvitationToken, string NewPassword) : IRequest<bool>;

public class SetPasswordCommandHandler : IRequestHandler<SetPasswordCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public SetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> Handle(SetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return false;

        // Verify the exact invitation token
        if (user.InvitationToken != request.InvitationToken) return false;
        
        // Ensure the token has not expired
        if (user.InvitationTokenExpiry < DateTime.UtcNow) return false;

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);

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
}
