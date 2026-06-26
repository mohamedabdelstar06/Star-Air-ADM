namespace StarAirAdm.Application.Features.Decide.Commands;

public record AddDecideStepCommand(int SessionId, CreateDecideStepDto Dto) : IRequest<DecideStepResponseDto?>;

public class AddDecideStepCommandHandler : IRequestHandler<AddDecideStepCommand, DecideStepResponseDto?>
{
    private readonly IApplicationDbContext _context;

    public AddDecideStepCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DecideStepResponseDto?> Handle(AddDecideStepCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.DecideSessions.FindAsync(new object[] { request.SessionId }, cancellationToken);
        if (session == null || session.Status != SessionStatus.InProgress) return null;

        var stepType = (DecideStepType)request.Dto.StepType;
        var suggested = GetSuggestedActions(stepType, request.Dto.Input);

        var step = new DecideStep
        {
            SessionId = request.SessionId,
            StepType = stepType,
            StepOrder = (int)stepType + 1,
            Input = request.Dto.Input,
            Notes = request.Dto.Notes,
            SuggestedActions = suggested,
            SelectedAction = request.Dto.SelectedAction,
            CompletedAt = DateTime.UtcNow
        };

        _context.DecideSteps.Add(step);
        await _context.SaveChangesAsync(cancellationToken);
        return MapStep(step);
    }

    private static string GetSuggestedActions(DecideStepType stepType, string? input)
    {
        return stepType switch
        {
            DecideStepType.Detect             => "[\"Cross-check instruments\",\"Scan external/internal domains\",\"Tick off PAVE corners\",\"Log emerging hazards on kneeboard\"]",
            DecideStepType.Evaluate           => "[\"Assess severity and likelihood\",\"Translate to simple risk score (low/med/high)\",\"Consider time pressure and endurance\"]",
            DecideStepType.Consider           => "[\"Brainstorm 3 viable mitigations\",\"Engage crew or passengers for input\",\"Write options on a scratchpad\"]",
            DecideStepType.Integrate          => "[\"Align actions with aircraft limits\",\"Factor external constraints (airspace/fuel)\",\"Sequence tasks into logical flow\"]",
            DecideStepType.Decide             => "[\"Commit to one course of action\",\"Brief passengers on maneuver\",\"Tell ATC, set radios and autopilot\"]",
            DecideStepType.ExecuteAndReassess => "[\"Execute precise control inputs\",\"Verify plan effect\",\"Run another PAVE scan in 5 mins\"]",
            _                                 => "[]"
        };
    }

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
