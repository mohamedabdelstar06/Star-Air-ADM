namespace StarAirAdm.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ApplicationUser> Users { get; set; }
    DbSet<ImSafeAssessment> ImSafeAssessments { get; }
    DbSet<PaveAssessment> PaveAssessments { get; }
    DbSet<DecideSession> DecideSessions { get; }
    DbSet<DecideStep> DecideSteps { get; }
    DbSet<SmartWatchReading> SmartWatchReadings { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<KneeboardNote> KneeboardNotes { get; }
    DbSet<WeatherCache> WeatherCaches { get; }
    DbSet<NotamCache> NotamCaches { get; }
    DbSet<Checklist> Checklists { get; }
    DbSet<ChecklistItem> ChecklistItems { get; }
    DbSet<FlightTrip> FlightTrips { get; }
    DbSet<Notification> Notifications { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
