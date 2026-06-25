using StarAirAdm.Application.DTOs.Auth;
namespace StarAirAdm.Application.Interfaces;
public interface IAuthService
{
    Task<CheckEmailResponseDto> CheckEmailStatusAsync(CheckEmailDto request);
    Task<TokenResponseDto?> LoginAsync(LoginDto loginDto);
    Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenDto model);
    Task<bool> SetPasswordAsync(SetPasswordDto model);
    Task<bool> ForgotPasswordAsync(string email);
    Task<bool> ResetPasswordAsync(ResetPasswordDto model);
}
