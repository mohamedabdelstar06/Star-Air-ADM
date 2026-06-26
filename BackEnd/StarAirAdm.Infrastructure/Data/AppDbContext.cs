namespace StarAirAdm.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Removed Aircraft DbSet
    public DbSet<ImSafeAssessment> ImSafeAssessments => Set<ImSafeAssessment>();
    public DbSet<PaveAssessment> PaveAssessments => Set<PaveAssessment>();
    public DbSet<DecideSession> DecideSessions => Set<DecideSession>();
    public DbSet<DecideStep> DecideSteps => Set<DecideStep>();
    public DbSet<SmartWatchReading> SmartWatchReadings => Set<SmartWatchReading>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<KneeboardNote> KneeboardNotes => Set<KneeboardNote>();
    public DbSet<WeatherCache> WeatherCaches => Set<WeatherCache>();
    public DbSet<NotamCache> NotamCaches => Set<NotamCache>();
    public DbSet<Checklist> Checklists => Set<Checklist>();
    public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();
    public DbSet<FlightTrip> FlightTrips => Set<FlightTrip>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // SmartWatchReading is the dependent side â€” FlightTripId is the FK
        builder.Entity<FlightTrip>()
            .HasOne(f => f.SmartWatchReading)
            .WithOne(s => s.FlightTrip)
            .HasForeignKey<SmartWatchReading>(s => s.FlightTripId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
