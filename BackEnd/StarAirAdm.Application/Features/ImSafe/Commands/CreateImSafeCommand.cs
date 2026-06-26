namespace StarAirAdm.Application.Features.ImSafe.Commands;

public record CreateImSafeCommand(CreateImSafeDto Dto, string PilotId) : IRequest<ImSafeResponseDto?>;

public class CreateImSafeCommandHandler : IRequestHandler<CreateImSafeCommand, ImSafeResponseDto?>
{
    private readonly IApplicationDbContext _context;

    public CreateImSafeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ImSafeResponseDto?> Handle(CreateImSafeCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var illness    = (RiskLevel)dto.IllnessLevel;
        var medication = (RiskLevel)dto.MedicationLevel;
        var stress     = (RiskLevel)dto.StressLevel;
        var alcohol    = (RiskLevel)dto.AlcoholLevel;
        var fatigue    = (RiskLevel)dto.FatigueLevel;
        var emotion    = (RiskLevel)dto.EmotionLevel;

        var (score, result) = RiskScoringEngine.ScoreImSafe(
            illness, medication, stress, alcohol, fatigue, emotion,
            dto.HoursSinceLastDrink, dto.HoursSlept);

        var assessment = new ImSafeAssessment
        {
            PilotId = request.PilotId,
            IllnessLevel = illness, IllnessNotes = dto.IllnessNotes,
            MedicationLevel = medication, MedicationNotes = dto.MedicationNotes,
            StressLevel = stress, StressNotes = dto.StressNotes,
            AlcoholLevel = alcohol, HoursSinceLastDrink = dto.HoursSinceLastDrink,
            FatigueLevel = fatigue, HoursSlept = dto.HoursSlept,
            EmotionLevel = emotion, EmotionNotes = dto.EmotionNotes,
            DataSource = (DataSource)dto.DataSource,
            OverallRiskScore = score,
            Result = result,
            AssessedAt = DateTime.UtcNow,
            IsSynced = dto.IsSynced
        };

        _context.ImSafeAssessments.Add(assessment);
        await _context.SaveChangesAsync(cancellationToken);

        var pilotModel = await _context.Users.FindAsync(new object[] { request.PilotId }, cancellationToken);
        if (pilotModel != null) assessment.Pilot = pilotModel;

        return Map(assessment);
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
