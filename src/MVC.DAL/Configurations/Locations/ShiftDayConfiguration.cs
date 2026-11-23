using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Locations;

namespace MVC.DAL.Configurations.Locations
{
    // جدول ShiftDays - Composite PK (ShiftId, DayOfWeek)
    public class ShiftDayConfiguration : IEntityTypeConfiguration<ShiftDay>
    {
        public void Configure(EntityTypeBuilder<ShiftDay> builder)
        {
            builder.ToTable("ShiftDays");

            builder.HasKey(sd => new { sd.ShiftId, sd.DayOfWeek });

            builder.HasOne(sd => sd.Shift)
                   .WithMany(s => s.ShiftDays)
                   .HasForeignKey(sd => sd.ShiftId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
