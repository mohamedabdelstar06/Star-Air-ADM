using Microsoft.AspNetCore.Identity;
using StarAirAdm.Application.DTOs.Users;
using StarAirAdm.Application.Interfaces;
using StarAirAdm.Domain.Entities;
using StarAirAdm.Domain.Enums;

namespace StarAirAdm.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;

    public UserService(UserManager<ApplicationUser> userManager, IEmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<(UserResponseDto? User, string? Error)> CreateUserAsync(CreateUserDto dto)
    {
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
            Status = UserStatus.Pending, // Pending until they set password
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
            // For now, silently proceed since user was created
        }

        var roles = await _userManager.GetRolesAsync(user);
        return (MapToDto(user, roles), null);
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        var users = _userManager.Users.ToList();
        var responseList = new List<UserResponseDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            responseList.Add(MapToDto(user, roles));
        }

        return responseList;
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return MapToDto(user, roles);
    }

    public async Task<UserResponseDto?> UpdateUserAsync(string id, UpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return null;

        user.FullName = dto.FullName;
        user.LicenseNumber = dto.LicenseNumber;
        user.MedicalClass = dto.MedicalClass;
        user.Rank = dto.Rank;
        user.TotalFlightHours = dto.TotalFlightHours;
        user.ProfileImageUrl = dto.ProfileImageUrl;
        user.UpdatedAt = DateTime.UtcNow;

        // Handle email update
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
        return MapToDto(user, roles);
    }

    public async Task<bool> ToggleUserStatusAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
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

    public async Task<bool> DeleteUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    private static UserResponseDto MapToDto(ApplicationUser user, IList<string> roles)
    {
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
