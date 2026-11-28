using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Locations;

namespace MVC.DAL.Configurations.Locations
{
    // ShiftExceptions table - unique on (ShiftId, ExceptionDate)
    public class ShiftExceptionConfiguration : IEntityTypeConfiguration<ShiftException>
    {
        public void Configure(EntityTypeBuilder<ShiftException> builder)
        {
            builder.ToTable("ShiftExceptions", t =>
            {
                // Blocked => no times - SpecialWorkingHours => both times required and the end after the start
                t.HasCheckConstraint("CK_ShiftExceptions_Times",
                    "([ExceptionType] = 'Blocked' AND [StartTime] IS NULL AND [EndTime] IS NULL) OR " +
                    "([ExceptionType] = 'SpecialWorkingHours' AND [StartTime] IS NOT NULL AND [EndTime] IS NOT NULL AND [EndTime] > [StartTime])");
            });

            builder.HasKey(e => e.ShiftExceptionId);

            builder.HasOne(e => e.Shift)
                   .WithMany(s => s.ShiftExceptions)
                   .HasForeignKey(e => e.ShiftId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => new { e.ShiftId, e.ExceptionDate })
                   .IsUnique();
        }
    }
}
