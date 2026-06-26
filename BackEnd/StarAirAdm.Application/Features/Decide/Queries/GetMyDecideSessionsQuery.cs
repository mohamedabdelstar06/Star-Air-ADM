namespace StarAirAdm.Application.Features.Decide.Queries;

public record GetMyDecideSessionsQuery(string PilotId) : IRequest<IEnumerable<DecideSessionResponseDto>>;

public class GetMyDecideSessionsQueryHandler : IRequestHandler<GetMyDecideSessionsQuery, IEnumerable<DecideSessionResponseDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMyDecideSessionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DecideSessionResponseDto>> Handle(GetMyDecideSessionsQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.DecideSessions
            .Include(s => s.Pilot).Include(s => s.Steps)
            .Where(s => s.PilotId == request.PilotId)
            .OrderByDescending(s => s.StartedAt)
            .AsNoTracking().ToListAsync(cancellationToken);
            
        return list.Select(Map);
    }

    private static DecideSessionResponseDto Map(DecideSession s) => new()
    {
        Id = s.Id,
        PilotId = s.PilotId,
        PilotName = s.Pilot?.FullName ?? string.Empty,
        Scenario = s.Scenario,
        Status = s.Status.ToString(),
        FinalRiskScore = s.FinalRiskScore,
        StartedAt = s.StartedAt,
        CompletedAt = s.CompletedAt,
        IsSynced = s.IsSynced,
        Steps = s.Steps.OrderBy(x => x.StepOrder).Select(MapStep).ToList()
    };

    private static DecideStepResponseDto MapStep(DecideStep s) => new()
    {
        Id = s.Id,
        SessionId = s.SessionId,
        StepType = s.StepType.ToString(),
        StepOrder = s.StepOrder,
        Input = s.Input,
        Notes = s.Notes,
        SuggestedActions = s.SuggestedActions,
        SelectedAction = s.SelectedAction,
        CompletedAt = s.CompletedAt
    };
}
