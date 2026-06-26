namespace StarAirAdm.Domain.Entities;

public class FlightTrip : BaseEntity
{
    public string PilotId { get; set; } = null!;
    public string FlightCategory { get; set; } = null!;
    public string AircraftType { get; set; } = null!;
    public string Departure { get; set; } = null!;
    public string Arrival { get; set; } = null!;
    public DateTime DepartureTime { get; set; }
    public string? FlightNumber { get; set; }

    // Status: Pending, Cleared, Completed
    public FlightStatus Status { get; set; } = FlightStatus.Pending;

    // Assessment Links (Force completion)
    public int? ImSafeAssessmentId { get; set; }
    public int? PaveAssessmentId { get; set; }
    public int? DecideSessionId { get; set; }
    public int? SmartWatchReadingId { get; set; }
    public string? WeatherRiskLevel { get; set; }   // Go / Caution / NoGo
    public string? WeatherSummary { get; set; }     // Human-readable summary

    // Navigation
    public ApplicationUser Pilot { get; set; } = null!;
    public ImSafeAssessment? ImSafeAssessment { get; set; }
    public PaveAssessment? PaveAssessment { get; set; }
    public DecideSession? DecideSession { get; set; }
    public SmartWatchReading? SmartWatchReading { get; set; }
}
