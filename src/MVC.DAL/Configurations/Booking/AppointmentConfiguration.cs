using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Booking;

namespace MVC.DAL.Configurations.Booking
{
    // Appointments table
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments", t =>
            {
                // XOR: exactly one of ShiftId and AdditionalShiftId
                t.HasCheckConstraint("CK_Appointments_ShiftTarget",
                    "([ShiftId] IS NOT NULL AND [AdditionalShiftId] IS NULL) OR ([ShiftId] IS NULL AND [AdditionalShiftId] IS NOT NULL)");
            });

            builder.HasKey(a => a.AppointmentId);

            builder.HasOne(a => a.Patient)
                   .WithMany(p => p.Appointments)
                   .HasForeignKey(a => a.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Doctor)
                   .WithMany(d => d.Appointments)
                   .HasForeignKey(a => a.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Shift)
                   .WithMany(s => s.Appointments)
                   .HasForeignKey(a => a.ShiftId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.AdditionalShift)
                   .WithMany(s => s.Appointments)
                   .HasForeignKey(a => a.AdditionalShiftId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.CancelledByUser)
                   .WithMany()
                   .HasForeignKey(a => a.CancelledByUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // No two non-cancelled appointments for the same patient and doctor on the same date
            builder.HasIndex(a => new { a.PatientId, a.DoctorId, a.AppointmentDate }, "UX_Appointments_Patient_Doctor_Date")
                   .IsUnique()
                   .HasFilter("[Status] <> 'Cancelled'");
        }
    }
}
