using StarAirAdm.Application.DTOs.AuditLog;

namespace StarAirAdm.Application.Interfaces;

public interface IAuditLogService
{
    Task<IEnumerable<AuditLogResponseDto>> GetAllLogsAsync();
}
