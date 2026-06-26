namespace StarAirAdm.Application.Features.Users.Queries;

public record GetUsersQuery() : IRequest<IEnumerable<UserResponseDto>>;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserResponseDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUsersQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<UserResponseDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = _userManager.Users.ToList();
        var responseList = new List<UserResponseDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            responseList.Add(new UserResponseDto
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
            });
        }

        return responseList;
    }
}
