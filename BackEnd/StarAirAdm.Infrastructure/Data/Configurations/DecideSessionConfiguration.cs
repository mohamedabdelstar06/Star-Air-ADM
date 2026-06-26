namespace StarAirAdm.Infrastructure.Data.Configurations;

public class DecideSessionConfiguration : IEntityTypeConfiguration<DecideSession>
{
    public void Configure(EntityTypeBuilder<DecideSession> builder)
    {
        builder.HasOne(d => d.Pilot)
            .WithMany(u => u.DecideSessions)
            .HasForeignKey(d => d.PilotId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(d => d.Status).HasConversion<string>().HasMaxLength(30);

        builder.HasMany(d => d.Steps)
            .WithOne(s => s.Session)
            .HasForeignKey(s => s.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class DecideStepConfiguration : IEntityTypeConfiguration<DecideStep>
{
    public void Configure(EntityTypeBuilder<DecideStep> builder)
    {
        builder.Property(s => s.StepType).HasConversion<string>().HasMaxLength(30);
    }
}
