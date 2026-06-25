namespace StarAirAdm.Application.DTOs.Auth;

    public class SetPasswordDto
    {
        public string Email { get; set; } = string.Empty;
        public string InvitationToken { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
