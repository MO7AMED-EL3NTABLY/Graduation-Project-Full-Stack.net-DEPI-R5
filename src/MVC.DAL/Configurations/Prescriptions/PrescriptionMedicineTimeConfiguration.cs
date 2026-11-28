using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Prescriptions;

namespace MVC.DAL.Configurations.Prescriptions
{
    // PrescriptionMedicineTimes table - composite PK (PrescriptionMedicineId, TimeOfDay)
    public class PrescriptionMedicineTimeConfiguration : IEntityTypeConfiguration<PrescriptionMedicineTime>
    {
        public void Configure(EntityTypeBuilder<PrescriptionMedicineTime> builder)
        {
            builder.ToTable("PrescriptionMedicineTimes");

            builder.HasKey(t => new { t.PrescriptionMedicineId, t.TimeOfDay });

            builder.HasOne(t => t.PrescriptionMedicine)
                   .WithMany(m => m.Times)
                   .HasForeignKey(t => t.PrescriptionMedicineId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
