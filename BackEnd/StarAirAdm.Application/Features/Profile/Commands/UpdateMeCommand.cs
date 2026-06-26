namespace StarAirAdm.Application.Features.Profile.Commands;

public record UpdateMeCommand(string UserId, UpdateUserDto Dto) : IRequest<UserResponseDto?>;

public class UpdateMeCommandHandler : IRequestHandler<UpdateMeCommand, UserResponseDto?>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateMeCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserResponseDto?> Handle(UpdateMeCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null) return null;

        var dto = request.Dto;
        user.FullName = dto.FullName;
        user.LicenseNumber = dto.LicenseNumber;
        user.MedicalClass = dto.MedicalClass;
        user.Rank = dto.Rank;
        user.TotalFlightHours = dto.TotalFlightHours;
        
        // We only update ProfileImageUrl if it's provided in the DTO, otherwise we keep the existing one
        if (!string.IsNullOrWhiteSpace(dto.ProfileImageUrl))
        {
            user.ProfileImageUrl = dto.ProfileImageUrl;
        }

        user.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
        {
            user.Email = dto.Email;
            user.NormalizedEmail = dto.Email.ToUpperInvariant();
            user.UserName = dto.Email;
            user.NormalizedUserName = dto.Email.ToUpperInvariant();
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return new UserResponseDto
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
        };
    }
}
