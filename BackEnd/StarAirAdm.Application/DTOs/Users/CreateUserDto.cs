namespace StarAirAdm.Application.DTOs.Users;

public class CreateUserDto
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // "Admin" or "Pilot"
    public string? LicenseNumber { get; set; }
    public string? MedicalClass { get; set; }
    public string? Rank { get; set; }
}
