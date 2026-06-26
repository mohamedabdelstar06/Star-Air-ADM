namespace StarAirAdm.Application.Features.Flights.Queries;

public record GetAllFlightsQuery() : IRequest<IEnumerable<FlightTripResponseDto>>;

public class GetAllFlightsQueryHandler : IRequestHandler<GetAllFlightsQuery, IEnumerable<FlightTripResponseDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllFlightsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FlightTripResponseDto>> Handle(GetAllFlightsQuery request, CancellationToken cancellationToken)
    {
        var flights = await _context.FlightTrips
            .Include(f => f.Pilot)
            .OrderByDescending(f => f.DepartureTime)
            .ToListAsync(cancellationToken);
            
        return flights.Select(MapToDto);
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
