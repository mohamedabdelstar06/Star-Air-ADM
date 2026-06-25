using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarAirAdm.Domain.Entities;

namespace StarAirAdm.Infrastructure.Data.Configurations;

public class PaveAssessmentConfiguration : IEntityTypeConfiguration<PaveAssessment>
{
    public void Configure(EntityTypeBuilder<PaveAssessment> builder)
    {
        builder.HasOne(p => p.Pilot)
            .WithMany(u => u.PaveAssessments)
            .HasForeignKey(p => p.PilotId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.Property(p => p.Result).HasConversion<string>().HasMaxLength(20);
    }
}
