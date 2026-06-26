namespace StarAirAdm.Application.Features.Pave.Commands;

public record CreatePaveCommand(CreatePaveDto Dto, string PilotId) : IRequest<PaveResponseDto?>;

public class CreatePaveCommandHandler : IRequestHandler<CreatePaveCommand, PaveResponseDto?>
{
    private readonly IApplicationDbContext _context;

    public CreatePaveCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaveResponseDto?> Handle(CreatePaveCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var pilot      = (RiskLevel)dto.PilotRiskLevel;
        var aircraft   = (RiskLevel)dto.AircraftRiskLevel;
        var env        = (RiskLevel)dto.EnvironmentRiskLevel;
        var external   = (RiskLevel)dto.ExternalRiskLevel;

        var (score, result) = RiskScoringEngine.ScorePave(pilot, aircraft, env, external);

        var assessment = new PaveAssessment
        {
            PilotId = request.PilotId,
            AircraftRegistration = dto.AircraftRegistration,
            PilotReadiness = dto.PilotReadiness, PilotRiskLevel = pilot,
            AircraftCondition = dto.AircraftCondition, AircraftRiskLevel = aircraft,
            WeatherSummary = dto.WeatherSummary, MetarData = dto.MetarData, TafData = dto.TafData,
            EnvironmentRiskLevel = env,
            ExternalPressures = dto.ExternalPressures, ExternalRiskLevel = external,
            OverallRiskScore = score,
            Result = result,
            AssessedAt = DateTime.UtcNow,
            IsSynced = dto.IsSynced
        };

        _context.PaveAssessments.Add(assessment);
        await _context.SaveChangesAsync(cancellationToken);

        var pilotModel = await _context.Users.FindAsync(new object[] { request.PilotId }, cancellationToken);
        if (pilotModel != null) assessment.Pilot = pilotModel;

        return Map(assessment);
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
