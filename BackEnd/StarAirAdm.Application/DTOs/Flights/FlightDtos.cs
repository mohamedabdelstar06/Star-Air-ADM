
namespace StarAirAdm.Application.DTOs.Flights;

public class CreateFlightTripDto
{
    public string PilotId { get; set; } = null!;
    public string FlightCategory { get; set; } = null!;
    public string AircraftType { get; set; } = null!;
    public string Departure { get; set; } = null!;
    public string Arrival { get; set; } = null!;
    public DateTime DepartureTime { get; set; }
    public string? FlightNumber { get; set; }
}

public class UpdateFlightTripDto
{
    public string? FlightCategory { get; set; }
    public string? AircraftType { get; set; }
    public string? Departure { get; set; }
    public string? Arrival { get; set; }
    public DateTime? DepartureTime { get; set; }
    public string? FlightNumber { get; set; }
    public string? Status { get; set; }
}

public class FlightTripResponseDto
{
    public int Id { get; set; }
    public string PilotId { get; set; } = null!;
    public string PilotName { get; set; } = string.Empty;
    public string FlightCategory { get; set; } = string.Empty;
    public string AircraftType { get; set; } = string.Empty;
    public string Departure { get; set; } = null!;
    public string Arrival { get; set; } = null!;
    public DateTime DepartureTime { get; set; }
    public string? FlightNumber { get; set; }
    public string Status { get; set; } = string.Empty;

    public int? ImSafeAssessmentId { get; set; }
    public int? PaveAssessmentId { get; set; }
    public int? DecideSessionId { get; set; }
    public int? SmartWatchReadingId { get; set; }    // Per-trip health data
    public string? WeatherRiskLevel { get; set; }    // Go / Caution / NoGo from weather
    public string? WeatherSummary { get; set; }      // Human-readable weather snapshot
    
    public DateTime CreatedAt { get; set; }
}

public class LinkAssessmentDto
{
    public int? ImSafeAssessmentId { get; set; }
    public int? PaveAssessmentId { get; set; }
    public int? DecideSessionId { get; set; }
    public int? SmartWatchReadingId { get; set; }
    public string? WeatherRiskLevel { get; set; }
    public string? WeatherSummary { get; set; }
}
