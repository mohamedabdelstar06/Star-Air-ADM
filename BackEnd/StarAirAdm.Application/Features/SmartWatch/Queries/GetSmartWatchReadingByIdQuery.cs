namespace StarAirAdm.Application.Features.SmartWatch.Queries;

public record GetSmartWatchReadingByIdQuery(int Id) : IRequest<SmartWatchReadingResponseDto?>;

public class GetSmartWatchReadingByIdQueryHandler : IRequestHandler<GetSmartWatchReadingByIdQuery, SmartWatchReadingResponseDto?>
{
    private readonly IApplicationDbContext _context;

    public GetSmartWatchReadingByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SmartWatchReadingResponseDto?> Handle(GetSmartWatchReadingByIdQuery request, CancellationToken cancellationToken)
    {
        var reading = await _context.SmartWatchReadings.FindAsync(new object[] { request.Id }, cancellationToken);
        return reading == null ? null : MapReading(reading);
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
