namespace StarAirAdm.Application.Features.ImSafe.Queries;

public record GetImSafeByIdQuery(int Id) : IRequest<ImSafeResponseDto?>;

public class GetImSafeByIdQueryHandler : IRequestHandler<GetImSafeByIdQuery, ImSafeResponseDto?>
{
    private readonly IApplicationDbContext _context;

    public GetImSafeByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ImSafeResponseDto?> Handle(GetImSafeByIdQuery request, CancellationToken cancellationToken)
    {
        var a = await _context.ImSafeAssessments
            .Include(x => x.Pilot)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            
        return a == null ? null : Map(a);
    }

    private static ImSafeResponseDto Map(ImSafeAssessment a) => new()
    {
        Id = a.Id,
        PilotId = a.PilotId,
        PilotName = a.Pilot?.FullName ?? string.Empty,
        IllnessLevel = a.IllnessLevel.ToString(),    IllnessNotes = a.IllnessNotes,
        MedicationLevel = a.MedicationLevel.ToString(), MedicationNotes = a.MedicationNotes,
        StressLevel = a.StressLevel.ToString(),      StressNotes = a.StressNotes,
        AlcoholLevel = a.AlcoholLevel.ToString(),    HoursSinceLastDrink = a.HoursSinceLastDrink,
        FatigueLevel = a.FatigueLevel.ToString(),    HoursSlept = a.HoursSlept,
        EmotionLevel = a.EmotionLevel.ToString(),    EmotionNotes = a.EmotionNotes,
        DataSource = a.DataSource.ToString(),
        OverallRiskScore = a.OverallRiskScore,
        Result = a.Result.ToString(),
        AssessedAt = a.AssessedAt,
        IsSynced = a.IsSynced
    };
}
