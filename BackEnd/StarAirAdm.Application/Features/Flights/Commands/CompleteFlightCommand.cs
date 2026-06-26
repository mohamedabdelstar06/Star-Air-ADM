namespace StarAirAdm.Application.Features.Flights.Commands;

public record CompleteFlightCommand(int FlightId) : IRequest<FlightTripResponseDto?>;

public class CompleteFlightCommandHandler : IRequestHandler<CompleteFlightCommand, FlightTripResponseDto?>
{
    private readonly IApplicationDbContext _context;

    public CompleteFlightCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<FlightTripResponseDto?> Handle(CompleteFlightCommand request, CancellationToken cancellationToken)
    {
        var flight = await _context.FlightTrips
            .Include(f => f.Pilot)
            .FirstOrDefaultAsync(f => f.Id == request.FlightId, cancellationToken);
            
        if (flight == null) return null;

        flight.Status = FlightStatus.Completed;
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(flight);
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
