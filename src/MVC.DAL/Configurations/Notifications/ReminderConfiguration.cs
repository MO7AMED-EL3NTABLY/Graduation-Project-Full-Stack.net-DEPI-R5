using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Notifications;

namespace MVC.DAL.Configurations.Notifications
{
    // Reminders table
    public class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
    {
        public void Configure(EntityTypeBuilder<Reminder> builder)
        {
            builder.ToTable("Reminders", t =>
            {
                // XOR: exactly one of AppointmentId and PrescriptionMedicineId
                t.HasCheckConstraint("CK_Reminders_Target",
                    "([AppointmentId] IS NOT NULL AND [PrescriptionMedicineId] IS NULL) OR ([AppointmentId] IS NULL AND [PrescriptionMedicineId] IS NOT NULL)");
            });

            builder.HasKey(r => r.ReminderId);

            builder.HasOne(r => r.Appointment)
                   .WithMany(a => a.Reminders)
                   .HasForeignKey(r => r.AppointmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.PrescriptionMedicine)
                   .WithMany(m => m.Reminders)
                   .HasForeignKey(r => r.PrescriptionMedicineId)
                   .OnDelete(DeleteBehavior.Restrict);

            // One non-cancelled reminder per appointment
            builder.HasIndex(r => r.AppointmentId, "UX_Reminders_Appointment")
                   .IsUnique()
                   .HasFilter("[AppointmentId] IS NOT NULL AND [Status] <> 'Cancelled'");

            // No two non-cancelled reminders for the same medicine and time
            builder.HasIndex(r => new { r.PrescriptionMedicineId, r.DueAtUtc }, "UX_Reminders_Medicine_Due")
                   .IsUnique()
                   .HasFilter("[PrescriptionMedicineId] IS NOT NULL AND [Status] <> 'Cancelled'");
        }
    }
}
