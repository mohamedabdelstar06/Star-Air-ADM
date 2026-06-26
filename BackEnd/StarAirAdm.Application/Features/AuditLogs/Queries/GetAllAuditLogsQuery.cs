namespace StarAirAdm.Application.Features.AuditLogs.Queries;

public record GetAllAuditLogsQuery() : IRequest<IEnumerable<AuditLogResponseDto>>;

public class GetAllAuditLogsQueryHandler : IRequestHandler<GetAllAuditLogsQuery, IEnumerable<AuditLogResponseDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllAuditLogsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AuditLogResponseDto>> Handle(GetAllAuditLogsQuery request, CancellationToken cancellationToken)
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
            .Take(100)
            .ToListAsync(cancellationToken);
    }
}
