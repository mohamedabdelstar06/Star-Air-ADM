namespace StarAirAdm.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? LicenseNumber { get; set; }
    public string? MedicalClass { get; set; }
    public DateTime? MedicalExpiry { get; set; }
    public string? Rank { get; set; }
    public int TotalFlightHours { get; set; }
    public string? ProfileImageUrl { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Pending;
    public string? InvitationToken { get; set; }
    public DateTime? InvitationTokenExpiry { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<ImSafeAssessment> ImSafeAssessments { get; set; } = new List<ImSafeAssessment>();
    public ICollection<PaveAssessment> PaveAssessments { get; set; } = new List<PaveAssessment>();
    public ICollection<DecideSession> DecideSessions { get; set; } = new List<DecideSession>();
    public ICollection<SmartWatchReading> SmartWatchReadings { get; set; } = new List<SmartWatchReading>();
    public ICollection<KneeboardNote> KneeboardNotes { get; set; } = new List<KneeboardNote>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
