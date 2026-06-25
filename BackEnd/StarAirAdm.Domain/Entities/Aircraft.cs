using StarAirAdm.Domain.Common;
using StarAirAdm.Domain.Enums;

namespace StarAirAdm.Domain.Entities;

public class Aircraft : BaseEntity
{
    public string RegistrationNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int? YearOfManufacture { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public AircraftStatus Status { get; set; } = AircraftStatus.Airworthy;
    // Navigation
    public ICollection<PaveAssessment> PaveAssessments { get; set; } = new List<PaveAssessment>();
}
