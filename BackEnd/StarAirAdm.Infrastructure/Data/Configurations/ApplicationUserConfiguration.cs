using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarAirAdm.Domain.Entities;

namespace StarAirAdm.Infrastructure.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FullName).HasMaxLength(150).IsRequired();
        builder.Property(u => u.LicenseNumber).HasMaxLength(50);
        builder.Property(u => u.MedicalClass).HasMaxLength(20);
        builder.Property(u => u.Rank).HasMaxLength(50);
        builder.Property(u => u.Status).IsRequired();
    }
}
