using Microsoft.EntityFrameworkCore;
using StarAirAdm.Application.DTOs.Flights;
using StarAirAdm.Application.Interfaces;
using StarAirAdm.Domain.Entities;
using StarAirAdm.Domain.Enums;
using StarAirAdm.Infrastructure.Data;

namespace StarAirAdm.Infrastructure.Services;

public class FlightService : IFlightService
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;
    private readonly INotificationService _notificationService;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

    public FlightService(AppDbContext context, IEmailService emailService, INotificationService notificationService, Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _context = context;
        _emailService = emailService;
        _notificationService = notificationService;
        _configuration = configuration;
    }

    public async Task<FlightTripResponseDto> CreateFlightAsync(CreateFlightTripDto dto)
    {
        var pilot = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.PilotId);
        if (pilot == null) throw new Exception("The selected pilot does not exist in the system.");

        var flight = new FlightTrip
        {
            PilotId = dto.PilotId,
            FlightCategory = dto.FlightCategory,
            AircraftType = dto.AircraftType,
            Departure = dto.Departure,
            Arrival = dto.Arrival,
            DepartureTime = dto.DepartureTime,
            FlightNumber = dto.FlightNumber,
            Status = FlightStatus.Pending
        };

        _context.FlightTrips.Add(flight);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Unwrap to the deepest inner exception to get the real SQL error message
            var inner = ex;
            while (inner.InnerException != null) inner = inner.InnerException;
            throw new Exception($"DB Save Failed: {inner.Message}");
        }

        var frontendUrl = _configuration["FrontendUrl"]?.TrimEnd('/') ?? "http://localhost:3000";
        var tripLink = $"{frontendUrl}/flights/{flight.Id}";

        // Send system notification to pilot
        var notifMessage = $"You have been assigned a new flight: {flight.FlightNumber ?? "N/A"}.";
        await _notificationService.CreateNotificationAsync(flight.PilotId, notifMessage, tripLink);

        // Send email notification to pilot
        if (!string.IsNullOrEmpty(pilot.Email))
        {
            var fTime = flight.DepartureTime.ToString("yyyy-MMM-dd HH:mm");
            var subject = $"New Trip Scheduled: {flight.FlightNumber ?? "Flight"}";
            var body = $@"
                <div style='font-family: sans-serif; padding: 20px;'>
                    <h2 style='color:#0f172a;'>Flight Assignment Notification</h2>
                    <p>Dear {pilot.FullName ?? "Pilot"},</p>
                    <p>You have been assigned to a new flight trip. Please log in to complete your pre-flight assessments.</p>
                    <div style='background-color:#f8fafc; padding:15px; border-radius:8px; margin:20px 0;'>
                        <ul style='list-style:none; padding:0; margin:0;'>
                            <li style='margin-bottom:10px;'><strong>Flight Number:</strong> {flight.FlightNumber ?? "N/A"}</li>
                            <li style='margin-bottom:10px;'><strong>Route:</strong> {flight.Departure} ➔ {flight.Arrival}</li>
                            <li style='margin-bottom:10px;'><strong>Departure Time:</strong> {fTime}</li>
                            <li style='margin-bottom:10px;'><strong>Aircraft:</strong> {flight.AircraftType}</li>
                            <li style='margin-bottom:0;'><strong>Category:</strong> {flight.FlightCategory}</li>
                        </ul>
                    </div>
                    <p>Please complete the IMSAFE, PAVE, and DECIDE workflows associated with this trip.</p>
                    <div style='margin-top:20px;'>
                        <a href='{tripLink}' style='background-color:#0284c7; color:white; padding:10px 20px; text-decoration:none; border-radius:5px; font-weight:bold;'>View Trip Details</a>
                    </div>
                    <p style='color:#64748b; font-size:12px; margin-top:30px;'>STAR Air Safety Management System</p>
                </div>
            ";
            try { await _emailService.SendEmailAsync(pilot.Email, subject, body); } catch { /* Ignore email failure */ }
        }

        return await GetFlightByIdAsync(flight.Id);
    }

    public async Task<IEnumerable<FlightTripResponseDto>> GetMyFlightsAsync(string pilotId)
    {
        var flights = await _context.FlightTrips
            .Include(f => f.Pilot)
            .Where(f => f.PilotId == pilotId)
            .OrderByDescending(f => f.DepartureTime)
            .ToListAsync();
            
        return flights.Select(MapToDto);
    }

    public async Task<IEnumerable<FlightTripResponseDto>> GetAllFlightsAsync()
    {
        var flights = await _context.FlightTrips
            .Include(f => f.Pilot)
            .OrderByDescending(f => f.DepartureTime)
            .ToListAsync();
            
        return flights.Select(MapToDto);
    }

    public async Task<FlightTripResponseDto> LinkAssessmentsAsync(int flightId, LinkAssessmentDto dto)
    {
        var flight = await _context.FlightTrips.FindAsync(flightId);
        if (flight == null) return null!;

        if (dto.ImSafeAssessmentId.HasValue) flight.ImSafeAssessmentId = dto.ImSafeAssessmentId;
        if (dto.PaveAssessmentId.HasValue) flight.PaveAssessmentId = dto.PaveAssessmentId;
        if (dto.DecideSessionId.HasValue) flight.DecideSessionId = dto.DecideSessionId;
        if (dto.SmartWatchReadingId.HasValue) flight.SmartWatchReadingId = dto.SmartWatchReadingId;
        if (!string.IsNullOrEmpty(dto.WeatherRiskLevel)) flight.WeatherRiskLevel = dto.WeatherRiskLevel;
        if (!string.IsNullOrEmpty(dto.WeatherSummary)) flight.WeatherSummary = dto.WeatherSummary;

        // Auto clearance if all 3 assessments are linked
        if (flight.ImSafeAssessmentId.HasValue && flight.PaveAssessmentId.HasValue && flight.DecideSessionId.HasValue)
        {
            flight.Status = FlightStatus.Cleared;
        }

        await _context.SaveChangesAsync();
        return await GetFlightByIdAsync(flight.Id);
    }

    public async Task<FlightTripResponseDto?> UpdateFlightAsync(int flightId, UpdateFlightTripDto dto)
    {
        var flight = await _context.FlightTrips.FindAsync(flightId);
        if (flight == null) return null;

        if (!string.IsNullOrEmpty(dto.FlightCategory)) flight.FlightCategory = dto.FlightCategory;
        if (!string.IsNullOrEmpty(dto.AircraftType)) flight.AircraftType = dto.AircraftType;
        if (!string.IsNullOrEmpty(dto.Departure)) flight.Departure = dto.Departure;
        if (!string.IsNullOrEmpty(dto.Arrival)) flight.Arrival = dto.Arrival;
        if (dto.DepartureTime.HasValue) flight.DepartureTime = dto.DepartureTime.Value;
        if (dto.FlightNumber != null) flight.FlightNumber = dto.FlightNumber;
        if (!string.IsNullOrEmpty(dto.Status) && Enum.TryParse<FlightStatus>(dto.Status, out var status))
            flight.Status = status;

        await _context.SaveChangesAsync();
        return await GetFlightByIdAsync(flight.Id);
    }

    public async Task<FlightTripResponseDto> CompleteFlightAsync(int flightId)
    {
        var flight = await _context.FlightTrips.FindAsync(flightId);
        if (flight == null) return null!;

        flight.Status = FlightStatus.Completed;
        await _context.SaveChangesAsync();

        return await GetFlightByIdAsync(flightId);
    }

    private async Task<FlightTripResponseDto> GetFlightByIdAsync(int id)
    {
        var f = await _context.FlightTrips
            .Include(f => f.Pilot)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        return f == null ? null! : MapToDto(f);
    }

    public async Task<bool> DeleteFlightAsync(int flightId)
    {
        var flight = await _context.FlightTrips.FindAsync(flightId);
        if(flight == null) return false;
        
        _context.FlightTrips.Remove(flight);
        await _context.SaveChangesAsync();
        return true;
    }

    private static FlightTripResponseDto MapToDto(FlightTrip f) => new FlightTripResponseDto
    {
        Id = f.Id,
        PilotId = f.PilotId,
        PilotName = f.Pilot?.FullName ?? "Unknown",
        FlightCategory = f.FlightCategory,
        AircraftType = f.AircraftType,
        Departure = f.Departure,
        Arrival = f.Arrival,
        DepartureTime = f.DepartureTime,
        FlightNumber = f.FlightNumber,
        Status = f.Status.ToString(),
        ImSafeAssessmentId = f.ImSafeAssessmentId,
        PaveAssessmentId = f.PaveAssessmentId,
        DecideSessionId = f.DecideSessionId,
        SmartWatchReadingId = f.SmartWatchReadingId,
        WeatherRiskLevel = f.WeatherRiskLevel,
        WeatherSummary = f.WeatherSummary,
        CreatedAt = f.CreatedAt
    };
}
