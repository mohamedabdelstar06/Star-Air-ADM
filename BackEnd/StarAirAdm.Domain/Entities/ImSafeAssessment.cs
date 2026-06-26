namespace StarAirAdm.Domain.Entities;

public class ImSafeAssessment : BaseEntity
{
    public string PilotId { get; set; } = string.Empty;

    // I - Illness
    public RiskLevel IllnessLevel { get; set; }
    public string? IllnessNotes { get; set; }

    // M - Medication
    public RiskLevel MedicationLevel { get; set; }
    public string? MedicationNotes { get; set; }

    // S - Stress
    public RiskLevel StressLevel { get; set; }
    public string? StressNotes { get; set; }

    // A - Alcohol
    public RiskLevel AlcoholLevel { get; set; }
    public double? HoursSinceLastDrink { get; set; }

    // F - Fatigue
    public RiskLevel FatigueLevel { get; set; }
    public double? HoursSlept { get; set; }

    // E - Emotion
    public RiskLevel EmotionLevel { get; set; }
    public string? EmotionNotes { get; set; }

    // Data source & results
    public DataSource DataSource { get; set; } = DataSource.Manual;
    public int OverallRiskScore { get; set; }
    public AssessmentResult Result { get; set; }
    public DateTime AssessedAt { get; set; } = DateTime.UtcNow;
    public bool IsSynced { get; set; } = true;

    // Navigation
    public ApplicationUser Pilot { get; set; } = null!;
}
