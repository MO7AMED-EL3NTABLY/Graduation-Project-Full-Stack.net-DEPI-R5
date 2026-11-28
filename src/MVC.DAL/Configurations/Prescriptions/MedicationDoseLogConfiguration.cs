using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Prescriptions;

namespace MVC.DAL.Configurations.Prescriptions
{
    // MedicationDoseLogs table
    public class MedicationDoseLogConfiguration : IEntityTypeConfiguration<MedicationDoseLog>
    {
        public void Configure(EntityTypeBuilder<MedicationDoseLog> builder)
        {
            builder.ToTable("MedicationDoseLogs");

            builder.HasKey(l => l.MedicationDoseLogId);

            builder.HasOne(l => l.PrescriptionMedicine)
                   .WithMany(m => m.DoseLogs)
                   .HasForeignKey(l => l.PrescriptionMedicineId)
                   .OnDelete(DeleteBehavior.Restrict);

            // One non-cancelled dose per medicine and time
            builder.HasIndex(l => new { l.PrescriptionMedicineId, l.ScheduledAtUtc }, "UX_MedicationDoseLogs_Medicine_Scheduled")
                   .IsUnique()
                   .HasFilter("[Status] <> 'Cancelled'");
        }
    }
}
