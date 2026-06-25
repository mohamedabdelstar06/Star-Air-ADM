using Microsoft.EntityFrameworkCore;
using StarAirAdm.Application.DTOs.Decide;
using StarAirAdm.Application.Interfaces;
using StarAirAdm.Domain.Entities;
using StarAirAdm.Domain.Enums;
using StarAirAdm.Infrastructure.Data;

namespace StarAirAdm.Infrastructure.Services;

public class DecideService : IDecideService
{
    private readonly AppDbContext _db;
    public DecideService(AppDbContext db) => _db = db;

    public async Task<DecideSessionResponseDto?> CreateSessionAsync(CreateDecideSessionDto dto, string pilotId)
    {
        var session = new DecideSession
        {
            PilotId = pilotId,
            Scenario = dto.Scenario,
            Status = SessionStatus.InProgress,
            StartedAt = DateTime.UtcNow
        };
        _db.DecideSessions.Add(session);
        await _db.SaveChangesAsync();
        await _db.Entry(session).Reference(s => s.Pilot).LoadAsync();
        return Map(session);
    }

    public async Task<IEnumerable<DecideSessionResponseDto>> GetByPilotAsync(string pilotId)
    {
        var list = await _db.DecideSessions
            .Include(s => s.Pilot).Include(s => s.Steps)
            .Where(s => s.PilotId == pilotId)
            .OrderByDescending(s => s.StartedAt)
            .AsNoTracking().ToListAsync();
        return list.Select(Map);
    }

    public async Task<IEnumerable<DecideSessionResponseDto>> GetAllAsync()
    {
        var list = await _db.DecideSessions
            .Include(s => s.Pilot).Include(s => s.Steps)
            .OrderByDescending(s => s.StartedAt)
            .AsNoTracking().ToListAsync();
        return list.Select(Map);
    }

    public async Task<DecideSessionResponseDto?> GetSessionByIdAsync(int id)
    {
        var s = await _db.DecideSessions
            .Include(x => x.Pilot).Include(x => x.Steps)
            .AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return s == null ? null : Map(s);
    }

    public async Task<DecideStepResponseDto?> AddStepAsync(int sessionId, CreateDecideStepDto dto)
    {
        var session = await _db.DecideSessions.FindAsync(sessionId);
        if (session == null || session.Status != SessionStatus.InProgress) return null;

        var stepType = (DecideStepType)dto.StepType;

        // Suggest actions based on step type
        var suggested = GetSuggestedActions(stepType, dto.Input);

        var step = new DecideStep
        {
            SessionId = sessionId,
            StepType = stepType,
            StepOrder = (int)stepType + 1,
            Input = dto.Input,
            Notes = dto.Notes,
            SuggestedActions = suggested,
            SelectedAction = dto.SelectedAction,
            CompletedAt = DateTime.UtcNow
        };

        _db.DecideSteps.Add(step);
        await _db.SaveChangesAsync();
        return MapStep(step);
    }

    public async Task<bool> CompleteSessionAsync(int sessionId, string pilotId)
    {
        var session = await _db.DecideSessions
            .Include(s => s.Steps)
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.PilotId == pilotId);

        if (session == null) return false;
        session.Status = SessionStatus.Completed;
        session.CompletedAt = DateTime.UtcNow;

        int completedSteps = session.Steps.Count;
        int stepsWithInput = session.Steps.Count(s => !string.IsNullOrWhiteSpace(s.Input));
        int missingInput = completedSteps - stepsWithInput;
        int skippedSteps = Math.Max(0, 6 - completedSteps);
        session.FinalRiskScore = (skippedSteps * 2) + missingInput;

        await _db.SaveChangesAsync();
        return true;
    }

    private static string GetSuggestedActions(DecideStepType stepType, string? input)
    {
        return stepType switch
        {
            DecideStepType.Detect             => "[\"Cross-check instruments\",\"Scan external/internal domains\",\"Tick off PAVE corners\",\"Log emerging hazards on kneeboard\"]",
            DecideStepType.Evaluate           => "[\"Assess severity and likelihood\",\"Translate to simple risk score (low/med/high)\",\"Consider time pressure and endurance\"]",
            DecideStepType.Consider           => "[\"Brainstorm 3 viable mitigations\",\"Engage crew or passengers for input\",\"Write options on a scratchpad\"]",
            DecideStepType.Integrate          => "[\"Align actions with aircraft limits\",\"Factor external constraints (airspace/fuel)\",\"Sequence tasks into logical flow\"]",
            DecideStepType.Decide             => "[\"Commit to one course of action\",\"Brief passengers on maneuver\",\"Tell ATC, set radios and autopilot\"]",
            DecideStepType.ExecuteAndReassess => "[\"Execute precise control inputs\",\"Verify plan effect\",\"Run another PAVE scan in 5 mins\"]",
            _                                 => "[]"
        };
    }

    private static DecideSessionResponseDto Map(DecideSession s) => new()
    {
        Id = s.Id,
        PilotId = s.PilotId,
        PilotName = s.Pilot?.FullName ?? string.Empty,
        Scenario = s.Scenario,
        Status = s.Status.ToString(),
        FinalRiskScore = s.FinalRiskScore,
        StartedAt = s.StartedAt,
        CompletedAt = s.CompletedAt,
        IsSynced = s.IsSynced,
        Steps = s.Steps.OrderBy(x => x.StepOrder).Select(MapStep).ToList()
    };

    private static DecideStepResponseDto MapStep(DecideStep s) => new()
    {
        Id = s.Id,
        SessionId = s.SessionId,
        StepType = s.StepType.ToString(),
        StepOrder = s.StepOrder,
        Input = s.Input,
        Notes = s.Notes,
        SuggestedActions = s.SuggestedActions,
        SelectedAction = s.SelectedAction,
        CompletedAt = s.CompletedAt
    };
}
