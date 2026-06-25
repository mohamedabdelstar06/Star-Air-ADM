using Microsoft.EntityFrameworkCore;
using StarAirAdm.Application.DTOs.Pave;
using StarAirAdm.Application.Interfaces;
using StarAirAdm.Domain.Entities;
using StarAirAdm.Domain.Enums;
using StarAirAdm.Infrastructure.Data;

namespace StarAirAdm.Infrastructure.Services;

public class PaveService : IPaveService
{
    private readonly AppDbContext _db;
    public PaveService(AppDbContext db) => _db = db;

    public async Task<PaveResponseDto?> CreateAsync(CreatePaveDto dto, string pilotId)
    {
        var pilot      = (RiskLevel)dto.PilotRiskLevel;
        var aircraft   = (RiskLevel)dto.AircraftRiskLevel;
        var env        = (RiskLevel)dto.EnvironmentRiskLevel;
        var external   = (RiskLevel)dto.ExternalRiskLevel;

        var (score, result) = RiskScoringEngine.ScorePave(pilot, aircraft, env, external);

        var assessment = new PaveAssessment
        {
            PilotId = pilotId,
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

        _db.PaveAssessments.Add(assessment);
        await _db.SaveChangesAsync();

        await _db.Entry(assessment).Reference(a => a.Pilot).LoadAsync();
        return Map(assessment);
    }

    public async Task<IEnumerable<PaveResponseDto>> GetByPilotAsync(string pilotId)
    {
        var list = await _db.PaveAssessments
            .Include(a => a.Pilot)
            .Where(a => a.PilotId == pilotId)
            .OrderByDescending(a => a.AssessedAt)
            .AsNoTracking().ToListAsync();
        return list.Select(Map);
    }

    public async Task<IEnumerable<PaveResponseDto>> GetAllAsync()
    {
        var list = await _db.PaveAssessments
            .Include(a => a.Pilot)
            .OrderByDescending(a => a.AssessedAt)
            .AsNoTracking().ToListAsync();
        return list.Select(Map);
    }

    public async Task<PaveResponseDto?> GetByIdAsync(int id)
    {
        var a = await _db.PaveAssessments
            .Include(x => x.Pilot)
            .AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return a == null ? null : Map(a);
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
