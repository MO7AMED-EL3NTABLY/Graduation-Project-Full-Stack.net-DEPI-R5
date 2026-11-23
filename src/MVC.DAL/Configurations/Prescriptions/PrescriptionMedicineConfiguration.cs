using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Prescriptions;

namespace MVC.DAL.Configurations.Prescriptions
{
    // جدول PrescriptionMedicines
    public class PrescriptionMedicineConfiguration : IEntityTypeConfiguration<PrescriptionMedicine>
    {
        public void Configure(EntityTypeBuilder<PrescriptionMedicine> builder)
        {
            builder.ToTable("PrescriptionMedicines", t =>
            {
                t.HasCheckConstraint("CK_PrescriptionMedicines_DurationDays", "[DurationDays] > 0");
            });

            builder.HasKey(m => m.PrescriptionMedicineId);

            builder.HasOne(m => m.Prescription)
                   .WithMany(p => p.Medicines)
                   .HasForeignKey(m => m.PrescriptionId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
