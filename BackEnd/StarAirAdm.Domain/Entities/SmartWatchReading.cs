using StarAirAdm.Domain.Common;

namespace StarAirAdm.Domain.Entities;

public class SmartWatchReading : BaseEntity
{
    public string PilotId { get; set; } = string.Empty;
    public int? FlightTripId { get; set; }          // Links reading to a specific trip
    public int? HeartRate { get; set; }
    public int? HeartRateVariability { get; set; }
    public double? SleepHours { get; set; }
    public int? SleepQuality { get; set; }       // 1-100
    public int? StressIndex { get; set; }         // 1-100
    public int? SpO2 { get; set; }                // percentage
    public double? SkinTemperature { get; set; }
    public int? Steps { get; set; }
    public string? DeviceName { get; set; }
    public string? RawData { get; set; }          // JSON
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    public bool IsSynced { get; set; } = true;
    public bool IsManualEntry { get; set; } = false; // true if pilot entered manually (no device)

    // Navigation
    public ApplicationUser Pilot { get; set; } = null!;
    public FlightTrip? FlightTrip { get; set; }
}
