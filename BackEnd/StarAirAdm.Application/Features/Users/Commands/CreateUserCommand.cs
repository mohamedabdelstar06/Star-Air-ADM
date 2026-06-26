namespace StarAirAdm.Application.Features.Users.Commands;

public record CreateUserCommand(CreateUserDto Dto) : IRequest<(UserResponseDto? User, string? Error)>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, (UserResponseDto? User, string? Error)>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;

    public CreateUserCommandHandler(UserManager<ApplicationUser> userManager, IEmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<(UserResponseDto? User, string? Error)> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null) return (null, "Email already exists in the system.");

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            LicenseNumber = dto.LicenseNumber,
            MedicalClass = dto.MedicalClass,
            Rank = dto.Rank,
            Status = UserStatus.Pending,
            InvitationToken = Guid.NewGuid().ToString("N"),
            InvitationTokenExpiry = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            return (null, $"Identity Error: {errors}");
        }

        string roleAssigned = (dto.Role?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true) ? "Admin" : "Pilot";
        await _userManager.AddToRoleAsync(user, roleAssigned);

        try 
        {
            await _emailService.SendWelcomeEmailAsync(user.Email, user.FullName, user.InvitationToken);
        }
        catch (Exception)
        {
            // Email sending failed, log it or handle accordingly
        }

        var roles = await _userManager.GetRolesAsync(user);
        return (new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles,
            LicenseNumber = user.LicenseNumber,
            MedicalClass = user.MedicalClass,
            Rank = user.Rank,
            TotalFlightHours = user.TotalFlightHours,
            ProfileImageUrl = user.ProfileImageUrl,
            Status = user.Status.ToString(),
            CreatedAt = user.CreatedAt
        }, null);
    }
}
