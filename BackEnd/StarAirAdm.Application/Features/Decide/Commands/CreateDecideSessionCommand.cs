namespace StarAirAdm.Application.Features.Decide.Commands;

public record CreateDecideSessionCommand(CreateDecideSessionDto Dto, string PilotId) : IRequest<DecideSessionResponseDto?>;

public class CreateDecideSessionCommandHandler : IRequestHandler<CreateDecideSessionCommand, DecideSessionResponseDto?>
{
    private readonly IApplicationDbContext _context;

    public CreateDecideSessionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DecideSessionResponseDto?> Handle(CreateDecideSessionCommand request, CancellationToken cancellationToken)
    {
        var session = new DecideSession
        {
            PilotId = request.PilotId,
            Scenario = request.Dto.Scenario,
            Status = SessionStatus.InProgress,
            StartedAt = DateTime.UtcNow
        };
        _context.DecideSessions.Add(session);
        await _context.SaveChangesAsync(cancellationToken);
        
        var pilot = await _context.Users.FindAsync(new object[] { request.PilotId }, cancellationToken);
        if (pilot != null) session.Pilot = pilot;

        return Map(session);
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
        Steps = new System.Collections.Generic.List<DecideStepResponseDto>()
    };
}
