namespace StarAirAdm.Application.Features.Decide.Queries;

public record GetDecideSessionByIdQuery(int Id) : IRequest<DecideSessionResponseDto?>;

public class GetDecideSessionByIdQueryHandler : IRequestHandler<GetDecideSessionByIdQuery, DecideSessionResponseDto?>
{
    private readonly IApplicationDbContext _context;

    public GetDecideSessionByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DecideSessionResponseDto?> Handle(GetDecideSessionByIdQuery request, CancellationToken cancellationToken)
    {
        var s = await _context.DecideSessions
            .Include(x => x.Pilot).Include(x => x.Steps)
            .AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            
        return s == null ? null : Map(s);
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
