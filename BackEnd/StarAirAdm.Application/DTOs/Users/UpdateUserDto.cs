namespace StarAirAdm.Application.DTOs.Users;

public class UpdateUserDto
{
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? LicenseNumber { get; set; }
    public string? MedicalClass { get; set; }
    public string? Rank { get; set; }
    public int TotalFlightHours { get; set; }
    public string? ProfileImageUrl { get; set; }
}
