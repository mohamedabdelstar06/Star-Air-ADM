namespace StarAirAdm.Application.Interfaces;

public interface IImSafeService
{
    Task<ImSafeResponseDto?> CreateAsync(CreateImSafeDto dto, string pilotId);
    Task<IEnumerable<ImSafeResponseDto>> GetByPilotAsync(string pilotId);
    Task<IEnumerable<ImSafeResponseDto>> GetAllAsync();
    Task<ImSafeResponseDto?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id, string requestingUserId);
}

public interface IPaveService
{
    Task<PaveResponseDto?> CreateAsync(CreatePaveDto dto, string pilotId);
    Task<IEnumerable<PaveResponseDto>> GetByPilotAsync(string pilotId);
    Task<IEnumerable<PaveResponseDto>> GetAllAsync();
    Task<PaveResponseDto?> GetByIdAsync(int id);
}

public interface IDecideService
{
    Task<DecideSessionResponseDto?> CreateSessionAsync(CreateDecideSessionDto dto, string pilotId);
    Task<IEnumerable<DecideSessionResponseDto>> GetByPilotAsync(string pilotId);
    Task<IEnumerable<DecideSessionResponseDto>> GetAllAsync();
    Task<DecideSessionResponseDto?> GetSessionByIdAsync(int id);
    Task<DecideStepResponseDto?> AddStepAsync(int sessionId, CreateDecideStepDto dto);
    Task<bool> CompleteSessionAsync(int sessionId, string pilotId);
}

public interface ISmartWatchService
{
    Task<SmartWatchReadingResponseDto?> AddReadingAsync(CreateSmartWatchReadingDto dto, string pilotId);
    Task<IEnumerable<SmartWatchReadingResponseDto>> GetByPilotAsync(string pilotId);
    Task<SmartWatchReadingResponseDto?> GetByIdAsync(int id);
    Task<SmartWatchAnalysisDto?> GetAnalysisAsync(string pilotId);
}

public interface IKneeboardService
{
    Task<KneeboardNoteResponseDto?> CreateAsync(CreateKneeboardNoteDto dto, string pilotId);
    Task<IEnumerable<KneeboardNoteResponseDto>> GetByPilotAsync(string pilotId);
    Task<KneeboardNoteResponseDto?> UpdateAsync(int id, CreateKneeboardNoteDto dto, string pilotId);
    Task<bool> DeleteAsync(int id, string pilotId);
}

public interface IChecklistService
{
    Task<ChecklistResponseDto?> CreateAsync(CreateChecklistDto dto, string createdBy);
    Task<IEnumerable<ChecklistResponseDto>> GetAllAsync();
    Task<ChecklistResponseDto?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
}

public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync();
}
