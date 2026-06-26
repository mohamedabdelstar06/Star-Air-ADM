namespace StarAirAdm.Application.Features.Profile.Queries;

public record GetMeQuery(string UserId) : IRequest<UserResponseDto?>;

public class GetMeQueryHandler : IRequestHandler<GetMeQuery, UserResponseDto?>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetMeQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserResponseDto?> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null) return null;

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
