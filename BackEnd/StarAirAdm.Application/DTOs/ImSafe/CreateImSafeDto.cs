namespace StarAirAdm.Application.DTOs.ImSafe;

public class CreateImSafeDto
{
    public int IllnessLevel { get; set; }
    public string? IllnessNotes { get; set; }

    public int MedicationLevel { get; set; }
    public string? MedicationNotes { get; set; }

    public int StressLevel { get; set; }
    public string? StressNotes { get; set; }

    public int AlcoholLevel { get; set; }
    public double? HoursSinceLastDrink { get; set; }

    public int FatigueLevel { get; set; }
    public double? HoursSlept { get; set; }

    public int EmotionLevel { get; set; }
    public string? EmotionNotes { get; set; }

    public int DataSource { get; set; } // 0=Manual, 1=SmartWatch
    public bool IsSynced { get; set; } = true;
}
