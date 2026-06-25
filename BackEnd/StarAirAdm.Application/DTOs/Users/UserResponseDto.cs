namespace StarAirAdm.Application.DTOs.Users;

public class UserResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = new List<string>();
    public string? LicenseNumber { get; set; }
    public string? MedicalClass { get; set; }
    public string? Rank { get; set; }
    public int TotalFlightHours { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
