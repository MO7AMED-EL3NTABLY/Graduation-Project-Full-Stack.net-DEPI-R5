using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Booking;

namespace MVC.DAL.Configurations.Booking
{
    // BookingHolds table
    public class BookingHoldConfiguration : IEntityTypeConfiguration<BookingHold>
    {
        public void Configure(EntityTypeBuilder<BookingHold> builder)
        {
            builder.ToTable("BookingHolds", t =>
            {
                // XOR: exactly one of ShiftId and AdditionalShiftId
                t.HasCheckConstraint("CK_BookingHolds_ShiftTarget",
                    "([ShiftId] IS NOT NULL AND [AdditionalShiftId] IS NULL) OR ([ShiftId] IS NULL AND [AdditionalShiftId] IS NOT NULL)");

                // NewBooking => no original appointment - Reschedule => original appointment required
                t.HasCheckConstraint("CK_BookingHolds_Purpose",
                    "([Purpose] = 'NewBooking' AND [OriginalAppointmentId] IS NULL) OR ([Purpose] = 'Reschedule' AND [OriginalAppointmentId] IS NOT NULL)");
            });

            builder.HasKey(h => h.BookingHoldId);

            builder.HasOne(h => h.Patient)
                   .WithMany(p => p.BookingHolds)
                   .HasForeignKey(h => h.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(h => h.Shift)
                   .WithMany(s => s.BookingHolds)
                   .HasForeignKey(h => h.ShiftId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(h => h.AdditionalShift)
                   .WithMany(s => s.BookingHolds)
                   .HasForeignKey(h => h.AdditionalShiftId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(h => h.OriginalAppointment)
                   .WithMany()
                   .HasForeignKey(h => h.OriginalAppointmentId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
