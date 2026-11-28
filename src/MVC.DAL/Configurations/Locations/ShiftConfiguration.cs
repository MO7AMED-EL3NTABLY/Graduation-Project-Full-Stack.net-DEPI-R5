using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Locations;

namespace MVC.DAL.Configurations.Locations
{
    // Shifts table
    public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
    {
        public void Configure(EntityTypeBuilder<Shift> builder)
        {
            builder.ToTable("Shifts", t =>
            {
                // No shift crosses midnight
                t.HasCheckConstraint("CK_Shifts_EndAfterStart", "[EndTime] > [StartTime]");
            });

            builder.HasKey(s => s.ShiftId);

            builder.HasOne(s => s.DoctorLocation)
                   .WithMany(dl => dl.Shifts)
                   .HasForeignKey(s => s.DoctorLocationId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
