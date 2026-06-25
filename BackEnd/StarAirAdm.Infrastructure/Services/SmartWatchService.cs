using Microsoft.EntityFrameworkCore;
using StarAirAdm.Application.DTOs.SmartWatch;
using StarAirAdm.Application.Interfaces;
using StarAirAdm.Domain.Entities;
using StarAirAdm.Infrastructure.Data;

namespace StarAirAdm.Infrastructure.Services;

public class SmartWatchService : ISmartWatchService
{
    private readonly AppDbContext _db;
    public SmartWatchService(AppDbContext db) => _db = db;

    public async Task<SmartWatchReadingResponseDto?> AddReadingAsync(CreateSmartWatchReadingDto dto, string pilotId)
    {
        var reading = new SmartWatchReading
        {
            PilotId = pilotId,
            FlightTripId = dto.FlightTripId,
            HeartRate = dto.HeartRate,
            HeartRateVariability = dto.HeartRateVariability,
            SleepHours = dto.SleepHours,
            SleepQuality = dto.SleepQuality,
            StressIndex = dto.StressIndex,
            SpO2 = dto.SpO2,
            SkinTemperature = dto.SkinTemperature,
            Steps = dto.Steps,
            DeviceName = dto.DeviceName,
            RawData = dto.RawData,
            RecordedAt = dto.RecordedAt ?? DateTime.UtcNow,
            IsSynced = dto.IsSynced,
            IsManualEntry = dto.IsManualEntry
        };

        _db.SmartWatchReadings.Add(reading);
        await _db.SaveChangesAsync();
        return MapReading(reading);
    }

    public async Task<IEnumerable<SmartWatchReadingResponseDto>> GetByPilotAsync(string pilotId)
    {
        var list = await _db.SmartWatchReadings
            .Where(r => r.PilotId == pilotId)
            .OrderByDescending(r => r.RecordedAt)
            .AsNoTracking().ToListAsync();
        return list.Select(MapReading);
    }

    public async Task<SmartWatchReadingResponseDto?> GetByIdAsync(int id)
    {
        var reading = await _db.SmartWatchReadings.FindAsync(id);
        return reading == null ? null : MapReading(reading);
    }

    public async Task<SmartWatchAnalysisDto?> GetAnalysisAsync(string pilotId)
    {
        // Take last 7 days of readings for analysis
        var since = DateTime.UtcNow.AddDays(-7);
        var readings = await _db.SmartWatchReadings
            .Where(r => r.PilotId == pilotId && r.RecordedAt >= since)
            .OrderByDescending(r => r.RecordedAt)
            .AsNoTracking().ToListAsync();

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
