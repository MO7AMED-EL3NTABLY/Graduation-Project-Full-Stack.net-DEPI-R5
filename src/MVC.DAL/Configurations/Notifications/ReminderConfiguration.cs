using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Notifications;

namespace MVC.DAL.Configurations.Notifications
{
    // جدول Reminders
    public class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
    {
        public void Configure(EntityTypeBuilder<Reminder> builder)
        {
            builder.ToTable("Reminders", t =>
            {
                // XOR: واحد بس من AppointmentId و PrescriptionMedicineId
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

            // تذكير واحد مش ملغي لكل حجز
            builder.HasIndex(r => r.AppointmentId, "UX_Reminders_Appointment")
                   .IsUnique()
                   .HasFilter("[AppointmentId] IS NOT NULL AND [Status] <> 'Cancelled'");

            // مفيش تذكيرين مش ملغيين لنفس الدوا ونفس الميعاد
            builder.HasIndex(r => new { r.PrescriptionMedicineId, r.DueAtUtc }, "UX_Reminders_Medicine_Due")
                   .IsUnique()
                   .HasFilter("[PrescriptionMedicineId] IS NOT NULL AND [Status] <> 'Cancelled'");
        }
    }
}
