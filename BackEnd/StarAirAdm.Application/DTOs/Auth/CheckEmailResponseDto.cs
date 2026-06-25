namespace StarAirAdm.Application.DTOs.Auth;

public class CheckEmailResponseDto
{
    public bool Exists { get; set; }
    public bool HasPassword { get; set; }
    public string Status { get; set; } = string.Empty; // Pending, Active, Inactive
}
