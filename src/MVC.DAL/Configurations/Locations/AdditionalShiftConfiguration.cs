using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Locations;

namespace MVC.DAL.Configurations.Locations
{
    // AdditionalShifts table
    public class AdditionalShiftConfiguration : IEntityTypeConfiguration<AdditionalShift>
    {
        public void Configure(EntityTypeBuilder<AdditionalShift> builder)
        {
            builder.ToTable("AdditionalShifts", t =>
            {
                // No shift crosses midnight
                t.HasCheckConstraint("CK_AdditionalShifts_EndAfterStart", "[EndTime] > [StartTime]");
            });

            builder.HasKey(s => s.AdditionalShiftId);

            builder.HasOne(s => s.DoctorLocation)
                   .WithMany(dl => dl.AdditionalShifts)
                   .HasForeignKey(s => s.DoctorLocationId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
