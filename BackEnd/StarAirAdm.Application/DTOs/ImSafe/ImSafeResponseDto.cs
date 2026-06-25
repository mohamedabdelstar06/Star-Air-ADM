namespace StarAirAdm.Application.DTOs.ImSafe;

public class ImSafeResponseDto
{
    public int Id { get; set; }
    public string PilotId { get; set; } = string.Empty;
    public string PilotName { get; set; } = string.Empty;

    public string IllnessLevel { get; set; } = string.Empty;
    public string? IllnessNotes { get; set; }

    public string MedicationLevel { get; set; } = string.Empty;
    public string? MedicationNotes { get; set; }

    public string StressLevel { get; set; } = string.Empty;
    public string? StressNotes { get; set; }

    public string AlcoholLevel { get; set; } = string.Empty;
    public double? HoursSinceLastDrink { get; set; }

    public string FatigueLevel { get; set; } = string.Empty;
    public double? HoursSlept { get; set; }

    public string EmotionLevel { get; set; } = string.Empty;
    public string? EmotionNotes { get; set; }

    public string DataSource { get; set; } = string.Empty;
    public int OverallRiskScore { get; set; }
    public string Result { get; set; } = string.Empty;
    public DateTime AssessedAt { get; set; }
    public bool IsSynced { get; set; }
}
