namespace StarAirAdm.Application.Features.Flights.Commands;

public record UpdateFlightCommand(int FlightId, UpdateFlightTripDto Dto) : IRequest<FlightTripResponseDto?>;

public class UpdateFlightCommandHandler : IRequestHandler<UpdateFlightCommand, FlightTripResponseDto?>
{
    private readonly IApplicationDbContext _context;

    public UpdateFlightCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<FlightTripResponseDto?> Handle(UpdateFlightCommand request, CancellationToken cancellationToken)
    {
        var flight = await _context.FlightTrips
            .Include(f => f.Pilot)
            .FirstOrDefaultAsync(f => f.Id == request.FlightId, cancellationToken);
            
        if (flight == null) return null;

        var dto = request.Dto;
        if (!string.IsNullOrEmpty(dto.FlightCategory)) flight.FlightCategory = dto.FlightCategory;
        if (!string.IsNullOrEmpty(dto.AircraftType)) flight.AircraftType = dto.AircraftType;
        if (!string.IsNullOrEmpty(dto.Departure)) flight.Departure = dto.Departure;
        if (!string.IsNullOrEmpty(dto.Arrival)) flight.Arrival = dto.Arrival;
        if (dto.DepartureTime.HasValue) flight.DepartureTime = dto.DepartureTime.Value;
        if (dto.FlightNumber != null) flight.FlightNumber = dto.FlightNumber;
        if (!string.IsNullOrEmpty(dto.Status) && Enum.TryParse<FlightStatus>(dto.Status, out var status))
            flight.Status = status;

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
