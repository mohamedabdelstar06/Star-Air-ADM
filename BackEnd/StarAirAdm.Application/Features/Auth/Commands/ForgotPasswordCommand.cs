namespace StarAirAdm.Application.Features.Auth.Commands;

public record ForgotPasswordCommand(string Email) : IRequest<bool>;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;

    public ForgotPasswordCommandHandler(UserManager<ApplicationUser> userManager, IEmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || user.Status != UserStatus.Active) return false;

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        
        try
        {
            await _emailService.SendPasswordResetEmailAsync(user.Email!, user.FullName ?? "", resetToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
