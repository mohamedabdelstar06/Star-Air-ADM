namespace StarAirAdm.Application.Features.Pave.Queries;

public record GetPaveByPilotQuery(string PilotId) : IRequest<IEnumerable<PaveResponseDto>>;

public class GetPaveByPilotQueryHandler : IRequestHandler<GetPaveByPilotQuery, IEnumerable<PaveResponseDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPaveByPilotQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PaveResponseDto>> Handle(GetPaveByPilotQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.PaveAssessments
            .Include(a => a.Pilot)
            .Where(a => a.PilotId == request.PilotId)
            .OrderByDescending(a => a.AssessedAt)
            .AsNoTracking().ToListAsync(cancellationToken);
            
        return list.Select(Map);
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
