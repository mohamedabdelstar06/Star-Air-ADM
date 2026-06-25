namespace StarAirAdm.Application.DTOs.Pave;

public class PaveResponseDto
{
    public int Id { get; set; }
    public string PilotId { get; set; } = string.Empty;
    public string PilotName { get; set; } = string.Empty;
    public string? AircraftRegistration { get; set; }

    public string? PilotReadiness { get; set; }
    public string PilotRiskLevel { get; set; } = string.Empty;

    public string? AircraftCondition { get; set; }
    public string AircraftRiskLevel { get; set; } = string.Empty;

    public string? WeatherSummary { get; set; }
    public string? MetarData { get; set; }
    public string? TafData { get; set; }
    public string EnvironmentRiskLevel { get; set; } = string.Empty;

    public string? ExternalPressures { get; set; }
    public string ExternalRiskLevel { get; set; } = string.Empty;

    public int OverallRiskScore { get; set; }
    public string Result { get; set; } = string.Empty;
    public DateTime AssessedAt { get; set; }
    public bool IsSynced { get; set; }
}
