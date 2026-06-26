namespace StarAirAdm.Application.Features.Flights.Commands;

public record CreateFlightCommand(CreateFlightTripDto Dto) : IRequest<FlightTripResponseDto>;

public class CreateFlightCommandHandler : IRequestHandler<CreateFlightCommand, FlightTripResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly INotificationService _notificationService;
    private readonly IConfiguration _configuration;

    public CreateFlightCommandHandler(
        IApplicationDbContext context, 
        IEmailService emailService, 
        INotificationService notificationService, 
        IConfiguration configuration)
    {
        _context = context;
        _emailService = emailService;
        _notificationService = notificationService;
        _configuration = configuration;
    }

    public async Task<FlightTripResponseDto> Handle(CreateFlightCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var pilot = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.PilotId, cancellationToken);
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
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            var inner = ex;
            while (inner.InnerException != null) inner = inner.InnerException;
            throw new Exception($"DB Save Failed: {inner.Message}");
        }

        var frontendUrl = _configuration["FrontendUrl"]?.TrimEnd('/') ?? "http://localhost:3000";
        var tripLink = $"{frontendUrl}/flights/{flight.Id}";

        var notifMessage = $"You have been assigned a new flight: {flight.FlightNumber ?? "N/A"}.";
        await _notificationService.CreateNotificationAsync(flight.PilotId, notifMessage, tripLink);

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
                            <li style='margin-bottom:10px;'><strong>Route:</strong> {flight.Departure} âž” {flight.Arrival}</li>
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
            try { await _emailService.SendEmailAsync(pilot.Email, subject, body); } catch { }
        }

        return MapToDto(flight, pilot);
    }

    private static FlightTripResponseDto MapToDto(FlightTrip f, ApplicationUser pilot) => new FlightTripResponseDto
    {
        Id = f.Id,
        PilotId = f.PilotId,
        PilotName = pilot.FullName ?? "Unknown",
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
