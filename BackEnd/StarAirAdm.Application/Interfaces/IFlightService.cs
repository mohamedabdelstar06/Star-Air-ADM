using StarAirAdm.Application.DTOs.Flights;

namespace StarAirAdm.Application.Interfaces;

public interface IFlightService
{
    Task<FlightTripResponseDto> CreateFlightAsync(CreateFlightTripDto dto);
    Task<FlightTripResponseDto?> UpdateFlightAsync(int flightId, UpdateFlightTripDto dto);
    Task<IEnumerable<FlightTripResponseDto>> GetMyFlightsAsync(string pilotId);
    Task<IEnumerable<FlightTripResponseDto>> GetAllFlightsAsync();
    Task<FlightTripResponseDto> LinkAssessmentsAsync(int flightId, LinkAssessmentDto dto);
    Task<FlightTripResponseDto> CompleteFlightAsync(int flightId);
    Task<bool> DeleteFlightAsync(int flightId);
}

