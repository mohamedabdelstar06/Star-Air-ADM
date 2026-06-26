namespace StarAirAdm.Application.Features.SmartWatch.Queries;

public record GetSmartWatchAnalysisQuery(string PilotId) : IRequest<SmartWatchAnalysisDto?>;

public class GetSmartWatchAnalysisQueryHandler : IRequestHandler<GetSmartWatchAnalysisQuery, SmartWatchAnalysisDto?>
{
    private readonly IApplicationDbContext _context;

    public GetSmartWatchAnalysisQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SmartWatchAnalysisDto?> Handle(GetSmartWatchAnalysisQuery request, CancellationToken cancellationToken)
    {
        var since = DateTime.UtcNow.AddDays(-7);
        var readings = await _context.SmartWatchReadings
            .Where(r => r.PilotId == request.PilotId && r.RecordedAt >= since)
            .OrderByDescending(r => r.RecordedAt)
            .AsNoTracking().ToListAsync(cancellationToken);

        if (!readings.Any()) return null;

        var latest = readings.First();
        var avgSleep = readings.Where(r => r.SleepHours.HasValue).Select(r => r.SleepHours!.Value).DefaultIfEmpty(0).Average();
        var avgStress = readings.Where(r => r.StressIndex.HasValue).Select(r => r.StressIndex!.Value).DefaultIfEmpty(0).Average();
        var avgSpO2 = readings.Where(r => r.SpO2.HasValue).Select(r => r.SpO2!.Value).DefaultIfEmpty(0).Average();

        var (riskScore, fitnessStatus, recommendation) = RiskScoringEngine.AnalyzeSmartWatch(
            latest.HeartRate, latest.HeartRateVariability,
            (int?)avgStress, (int?)avgSpO2,
            avgSleep, latest.SleepQuality
        );

        return new SmartWatchAnalysisDto
        {
            LatestHeartRate = latest.HeartRate,
            AverageSleepHours = Math.Round(avgSleep, 1),
            AverageStressIndex = (int)avgStress,
            AverageSpO2 = (int)avgSpO2,
            FitnessStatus = fitnessStatus,
            Recommendation = recommendation,
            RiskScore = riskScore
        };
    }
}
