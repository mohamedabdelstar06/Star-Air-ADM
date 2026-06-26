namespace StarAirAdm.Application.Features.ImSafe.Queries;

public record GetAllImSafeQuery() : IRequest<IEnumerable<ImSafeResponseDto>>;

public class GetAllImSafeQueryHandler : IRequestHandler<GetAllImSafeQuery, IEnumerable<ImSafeResponseDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllImSafeQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ImSafeResponseDto>> Handle(GetAllImSafeQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.ImSafeAssessments
            .Include(a => a.Pilot)
            .OrderByDescending(a => a.AssessedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
            
        return list.Select(Map);
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
