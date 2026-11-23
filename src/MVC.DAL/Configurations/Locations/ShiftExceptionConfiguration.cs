using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Locations;

namespace MVC.DAL.Configurations.Locations
{
    // جدول ShiftExceptions - Unique على (ShiftId, ExceptionDate)
    public class ShiftExceptionConfiguration : IEntityTypeConfiguration<ShiftException>
    {
        public void Configure(EntityTypeBuilder<ShiftException> builder)
        {
            builder.ToTable("ShiftExceptions");

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
