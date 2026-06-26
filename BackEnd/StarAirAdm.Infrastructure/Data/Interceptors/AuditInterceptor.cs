namespace StarAirAdm.Infrastructure.Data.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    public AuditInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        var userId = _currentUserService.UserId;
        var ipAddress = _currentUserService.IpAddress;
        
        var auditLogs = new List<AuditLog>();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = entry.State.ToString(),
                EntityType = entry.Entity.GetType().Name,
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow
            };

            var oldValues = new Dictionary<string, object?>();
            var newValues = new Dictionary<string, object?>();

            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary) continue;
                
                string propName = property.Metadata.Name;

                if (property.Metadata.IsPrimaryKey())
                {
                    auditLog.EntityId = property.CurrentValue?.ToString();
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        newValues[propName] = property.CurrentValue;
                        break;
                    case EntityState.Deleted:
                        oldValues[propName] = property.OriginalValue;
                        break;
                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            oldValues[propName] = property.OriginalValue;
                            newValues[propName] = property.CurrentValue;
                        }
                        break;
                }
            }

            if (oldValues.Count > 0)
                auditLog.OldValues = JsonSerializer.Serialize(oldValues);
            if (newValues.Count > 0)
                auditLog.NewValues = JsonSerializer.Serialize(newValues);

            auditLogs.Add(auditLog);
        }

        context.Set<AuditLog>().AddRange(auditLogs);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
