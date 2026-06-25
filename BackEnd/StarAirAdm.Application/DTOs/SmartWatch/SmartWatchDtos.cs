namespace StarAirAdm.Application.DTOs.SmartWatch;

public class CreateSmartWatchReadingDto
{
    public int? FlightTripId { get; set; }      // Link to specific trip
    public int? HeartRate { get; set; }
    public int? HeartRateVariability { get; set; }
    public double? SleepHours { get; set; }
    public int? SleepQuality { get; set; }
    public int? StressIndex { get; set; }
    public int? SpO2 { get; set; }
    public double? SkinTemperature { get; set; }
    public int? Steps { get; set; }
    public string? DeviceName { get; set; }
    public string? RawData { get; set; }
    public DateTime? RecordedAt { get; set; }
    public bool IsSynced { get; set; } = true;
    public bool IsManualEntry { get; set; } = false;
}

public class SmartWatchReadingResponseDto
{
    public int Id { get; set; }
    public string PilotId { get; set; } = string.Empty;
    public int? FlightTripId { get; set; }
    public int? HeartRate { get; set; }
    public int? HeartRateVariability { get; set; }
    public double? SleepHours { get; set; }
    public int? SleepQuality { get; set; }
    public int? StressIndex { get; set; }
    public int? SpO2 { get; set; }
    public double? SkinTemperature { get; set; }
    public int? Steps { get; set; }
    public string? DeviceName { get; set; }
    public DateTime RecordedAt { get; set; }
    public bool IsSynced { get; set; }
    public bool IsManualEntry { get; set; }
}

public class SmartWatchAnalysisDto
{
    public int? LatestHeartRate { get; set; }
    public double? AverageSleepHours { get; set; }
    public int? AverageStressIndex { get; set; }
    public int? AverageSpO2 { get; set; }
    public string FitnessStatus { get; set; } = string.Empty;  // Fit, Caution, Not Fit
    public string Recommendation { get; set; } = string.Empty;
    public int RiskScore { get; set; }
}
