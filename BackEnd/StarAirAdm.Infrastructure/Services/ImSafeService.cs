using Microsoft.EntityFrameworkCore;
using StarAirAdm.Application.DTOs.ImSafe;
using StarAirAdm.Application.Interfaces;
using StarAirAdm.Domain.Entities;
using StarAirAdm.Domain.Enums;
using StarAirAdm.Infrastructure.Data;

namespace StarAirAdm.Infrastructure.Services;

public class ImSafeService : IImSafeService
{
    private readonly AppDbContext _db;
    public ImSafeService(AppDbContext db) => _db = db;

    public async Task<ImSafeResponseDto?> CreateAsync(CreateImSafeDto dto, string pilotId)
    {
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
            PilotId = pilotId,
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

        _db.ImSafeAssessments.Add(assessment);
        await _db.SaveChangesAsync();

        await _db.Entry(assessment).Reference(a => a.Pilot).LoadAsync();
        return Map(assessment);
    }

    public async Task<IEnumerable<ImSafeResponseDto>> GetByPilotAsync(string pilotId)
    {
        var list = await _db.ImSafeAssessments
            .Include(a => a.Pilot)
            .Where(a => a.PilotId == pilotId)
            .OrderByDescending(a => a.AssessedAt)
            .AsNoTracking().ToListAsync();
        return list.Select(Map);
    }

    public async Task<IEnumerable<ImSafeResponseDto>> GetAllAsync()
    {
        var list = await _db.ImSafeAssessments
            .Include(a => a.Pilot)
            .OrderByDescending(a => a.AssessedAt)
            .AsNoTracking().ToListAsync();
        return list.Select(Map);
    }

    public async Task<ImSafeResponseDto?> GetByIdAsync(int id)
    {
        var a = await _db.ImSafeAssessments.Include(x => x.Pilot).AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        return a == null ? null : Map(a);
    }

    public async Task<bool> DeleteAsync(int id, string requestingUserId)
    {
        var a = await _db.ImSafeAssessments.FindAsync(id);
        if (a == null) return false;
        // Pilot can only delete own; admin can delete any (check is done in controller)
        _db.ImSafeAssessments.Remove(a);
        await _db.SaveChangesAsync();
        return true;
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
