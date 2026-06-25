namespace StarAirAdm.Application.DTOs.Pave;

public class CreatePaveDto
{
    public string? AircraftRegistration { get; set; }

    public string? PilotReadiness { get; set; }
    public int PilotRiskLevel { get; set; }

    public string? AircraftCondition { get; set; }
    public int AircraftRiskLevel { get; set; }

    public string? WeatherSummary { get; set; }
    public string? MetarData { get; set; }
    public string? TafData { get; set; }
    public int EnvironmentRiskLevel { get; set; }

    public string? ExternalPressures { get; set; }
    public int ExternalRiskLevel { get; set; }

    public bool IsSynced { get; set; } = true;
}
