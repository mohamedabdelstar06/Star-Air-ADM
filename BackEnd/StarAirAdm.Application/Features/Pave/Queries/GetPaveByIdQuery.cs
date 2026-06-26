namespace StarAirAdm.Application.Features.Pave.Queries;

public record GetPaveByIdQuery(int Id) : IRequest<PaveResponseDto?>;

public class GetPaveByIdQueryHandler : IRequestHandler<GetPaveByIdQuery, PaveResponseDto?>
{
    private readonly IApplicationDbContext _context;

    public GetPaveByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaveResponseDto?> Handle(GetPaveByIdQuery request, CancellationToken cancellationToken)
    {
        var a = await _context.PaveAssessments
            .Include(x => x.Pilot)
            .AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            
        return a == null ? null : Map(a);
    }

    private static PaveResponseDto Map(PaveAssessment a) => new()
    {
        Id = a.Id,
        PilotId = a.PilotId,
        PilotName = a.Pilot?.FullName ?? string.Empty,
        AircraftRegistration = a.AircraftRegistration,
        PilotReadiness = a.PilotReadiness, PilotRiskLevel = a.PilotRiskLevel.ToString(),
        AircraftCondition = a.AircraftCondition, AircraftRiskLevel = a.AircraftRiskLevel.ToString(),
        WeatherSummary = a.WeatherSummary, MetarData = a.MetarData, TafData = a.TafData,
        EnvironmentRiskLevel = a.EnvironmentRiskLevel.ToString(),
        ExternalPressures = a.ExternalPressures, ExternalRiskLevel = a.ExternalRiskLevel.ToString(),
        OverallRiskScore = a.OverallRiskScore,
        Result = a.Result.ToString(),
        AssessedAt = a.AssessedAt,
        IsSynced = a.IsSynced
    };
}
