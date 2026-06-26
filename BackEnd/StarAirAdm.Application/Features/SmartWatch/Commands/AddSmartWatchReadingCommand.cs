namespace StarAirAdm.Application.Features.SmartWatch.Commands;

public record AddSmartWatchReadingCommand(CreateSmartWatchReadingDto Dto, string PilotId) : IRequest<SmartWatchReadingResponseDto?>;

public class AddSmartWatchReadingCommandHandler : IRequestHandler<AddSmartWatchReadingCommand, SmartWatchReadingResponseDto?>
{
    private readonly IApplicationDbContext _context;

    public AddSmartWatchReadingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SmartWatchReadingResponseDto?> Handle(AddSmartWatchReadingCommand request, CancellationToken cancellationToken)
    {
        var reading = new SmartWatchReading
        {
            PilotId = request.PilotId,
            FlightTripId = request.Dto.FlightTripId,
            HeartRate = request.Dto.HeartRate,
            HeartRateVariability = request.Dto.HeartRateVariability,
            SleepHours = request.Dto.SleepHours,
            SleepQuality = request.Dto.SleepQuality,
            StressIndex = request.Dto.StressIndex,
            SpO2 = request.Dto.SpO2,
            SkinTemperature = request.Dto.SkinTemperature,
            Steps = request.Dto.Steps,
            DeviceName = request.Dto.DeviceName,
            RawData = request.Dto.RawData,
            RecordedAt = request.Dto.RecordedAt ?? DateTime.UtcNow,
            IsSynced = request.Dto.IsSynced,
            IsManualEntry = request.Dto.IsManualEntry
        };

        _context.SmartWatchReadings.Add(reading);
        await _context.SaveChangesAsync(cancellationToken);
        
        return MapReading(reading);
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
