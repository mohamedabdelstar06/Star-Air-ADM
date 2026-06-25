using Microsoft.EntityFrameworkCore;
using StarAirAdm.Application.DTOs.AuditLog;
using StarAirAdm.Application.Interfaces;
using StarAirAdm.Infrastructure.Data;

namespace StarAirAdm.Infrastructure.Services;

public class AuditLogService : IAuditLogService
{
    private readonly AppDbContext _context;

    public AuditLogService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AuditLogResponseDto>> GetAllLogsAsync()
    {
        return await _context.AuditLogs
            .Include(a => a.User)
            .OrderByDescending(a => a.Timestamp)
            .Select(a => new AuditLogResponseDto
            {
                Id = a.Id,
                UserId = a.UserId,
                UserName = a.User != null ? a.User.FullName : "System",
                Action = a.Action,
                EntityType = a.EntityType,
                EntityId = a.EntityId,
                OldValues = a.OldValues,
                NewValues = a.NewValues,
                IpAddress = a.IpAddress,
                Timestamp = a.Timestamp
            })
            .Take(100) // limit to recent 100 for performance
            .ToListAsync();
    }
}
