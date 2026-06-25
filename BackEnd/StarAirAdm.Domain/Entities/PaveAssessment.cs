using StarAirAdm.Domain.Common;
using StarAirAdm.Domain.Enums;

namespace StarAirAdm.Domain.Entities;

public class PaveAssessment : BaseEntity
{
    public string PilotId { get; set; } = string.Empty;
    public string? AircraftRegistration { get; set; }

    // P - Pilot
    public string? PilotReadiness { get; set; }
    public RiskLevel PilotRiskLevel { get; set; }

    // A - Aircraft
    public string? AircraftCondition { get; set; }
    public RiskLevel AircraftRiskLevel { get; set; }

    // V - enVironment
    public string? WeatherSummary { get; set; }
    public string? MetarData { get; set; }
    public string? TafData { get; set; }
    public RiskLevel EnvironmentRiskLevel { get; set; }

    // E - External Pressures
    public string? ExternalPressures { get; set; }
    public RiskLevel ExternalRiskLevel { get; set; }

    // Results
    public int OverallRiskScore { get; set; }
    public AssessmentResult Result { get; set; }
    public DateTime AssessedAt { get; set; } = DateTime.UtcNow;
    public bool IsSynced { get; set; } = true;

    // Navigation
    public ApplicationUser Pilot { get; set; } = null!;
}
