namespace StarAirAdm.Application.Features.Flights.Commands;

public record LinkAssessmentsCommand(int FlightId, LinkAssessmentDto Dto) : IRequest<FlightTripResponseDto?>;

public class LinkAssessmentsCommandHandler : IRequestHandler<LinkAssessmentsCommand, FlightTripResponseDto?>
{
    private readonly IApplicationDbContext _context;

    public LinkAssessmentsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<FlightTripResponseDto?> Handle(LinkAssessmentsCommand request, CancellationToken cancellationToken)
    {
        var flight = await _context.FlightTrips
            .Include(f => f.Pilot)
            .FirstOrDefaultAsync(f => f.Id == request.FlightId, cancellationToken);
            
        if (flight == null) return null;

        var dto = request.Dto;
        if (dto.ImSafeAssessmentId.HasValue) flight.ImSafeAssessmentId = dto.ImSafeAssessmentId;
        if (dto.PaveAssessmentId.HasValue) flight.PaveAssessmentId = dto.PaveAssessmentId;
        if (dto.DecideSessionId.HasValue) flight.DecideSessionId = dto.DecideSessionId;
        if (dto.SmartWatchReadingId.HasValue) flight.SmartWatchReadingId = dto.SmartWatchReadingId;
        if (!string.IsNullOrEmpty(dto.WeatherRiskLevel)) flight.WeatherRiskLevel = dto.WeatherRiskLevel;
        if (!string.IsNullOrEmpty(dto.WeatherSummary)) flight.WeatherSummary = dto.WeatherSummary;

        if (flight.ImSafeAssessmentId.HasValue && flight.PaveAssessmentId.HasValue && flight.DecideSessionId.HasValue)
        {
            flight.Status = FlightStatus.Cleared;
        }

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
