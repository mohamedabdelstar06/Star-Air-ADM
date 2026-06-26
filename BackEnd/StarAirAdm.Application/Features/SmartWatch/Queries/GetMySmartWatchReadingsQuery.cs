namespace StarAirAdm.Application.Features.SmartWatch.Queries;

public record GetMySmartWatchReadingsQuery(string PilotId) : IRequest<IEnumerable<SmartWatchReadingResponseDto>>;

public class GetMySmartWatchReadingsQueryHandler : IRequestHandler<GetMySmartWatchReadingsQuery, IEnumerable<SmartWatchReadingResponseDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMySmartWatchReadingsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SmartWatchReadingResponseDto>> Handle(GetMySmartWatchReadingsQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.SmartWatchReadings
            .Where(r => r.PilotId == request.PilotId)
            .OrderByDescending(r => r.RecordedAt)
            .AsNoTracking().ToListAsync(cancellationToken);
        return list.Select(MapReading);
    }

    private static SmartWatchReadingResponseDto MapReading(SmartWatchReading r) => new()
    {
        Id = r.Id,
        PilotId = r.PilotId,
        FlightTripId = r.FlightTripId,
        HeartRate = r.HeartRate,
        HeartRateVariability = r.HeartRateVariability,
        SleepHours = r.SleepHours,
        SleepQuality = r.SleepQuality,
        StressIndex = r.StressIndex,
        SpO2 = r.SpO2,
        SkinTemperature = r.SkinTemperature,
        Steps = r.Steps,
        DeviceName = r.DeviceName,
        RecordedAt = r.RecordedAt,
        IsSynced = r.IsSynced,
        IsManualEntry = r.IsManualEntry
    };
}
