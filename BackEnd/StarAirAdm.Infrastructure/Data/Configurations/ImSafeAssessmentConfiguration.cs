using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarAirAdm.Domain.Entities;

namespace StarAirAdm.Infrastructure.Data.Configurations;

public class ImSafeAssessmentConfiguration : IEntityTypeConfiguration<ImSafeAssessment>
{
    public void Configure(EntityTypeBuilder<ImSafeAssessment> builder)
    {
        builder.HasOne(i => i.Pilot)
            .WithMany(u => u.ImSafeAssessments)
            .HasForeignKey(i => i.PilotId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(i => i.Result).HasConversion<string>().HasMaxLength(20);
        builder.Property(i => i.DataSource).HasConversion<string>().HasMaxLength(20);
    }
}
