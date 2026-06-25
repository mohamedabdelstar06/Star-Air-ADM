using StarAirAdm.Application.DTOs.Users;

namespace StarAirAdm.Application.Interfaces;

public interface IUserService
{
    Task<(UserResponseDto? User, string? Error)> CreateUserAsync(CreateUserDto dto);
    Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    Task<UserResponseDto?> GetUserByIdAsync(string id);
    Task<UserResponseDto?> UpdateUserAsync(string id, UpdateUserDto dto);
    Task<bool> ToggleUserStatusAsync(string id);
    Task<bool> DeleteUserAsync(string id);
}
