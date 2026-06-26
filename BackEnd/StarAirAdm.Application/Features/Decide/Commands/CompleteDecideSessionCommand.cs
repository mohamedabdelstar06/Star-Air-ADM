namespace StarAirAdm.Application.Features.Decide.Commands;

public record CompleteDecideSessionCommand(int SessionId, string PilotId) : IRequest<bool>;

public class CompleteDecideSessionCommandHandler : IRequestHandler<CompleteDecideSessionCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public CompleteDecideSessionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CompleteDecideSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.DecideSessions
            .Include(s => s.Steps)
            .FirstOrDefaultAsync(s => s.Id == request.SessionId && s.PilotId == request.PilotId, cancellationToken);

        if (session == null) return false;
        
        session.Status = SessionStatus.Completed;
        session.CompletedAt = DateTime.UtcNow;

        int completedSteps = session.Steps.Count;
        int stepsWithInput = session.Steps.Count(s => !string.IsNullOrWhiteSpace(s.Input));
        int missingInput = completedSteps - stepsWithInput;
        int skippedSteps = Math.Max(0, 6 - completedSteps);
        session.FinalRiskScore = (skippedSteps * 2) + missingInput;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
