using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Prescriptions;

namespace MVC.DAL.Configurations.Prescriptions
{
    // جدول PrescriptionConditions - Composite PK و FKs من غير cascade
    public class PrescriptionConditionConfiguration : IEntityTypeConfiguration<PrescriptionCondition>
    {
        public void Configure(EntityTypeBuilder<PrescriptionCondition> builder)
        {
            builder.ToTable("PrescriptionConditions");

            builder.HasKey(pc => new { pc.PrescriptionId, pc.PatientConditionId });

            builder.HasOne(pc => pc.Prescription)
                   .WithMany(p => p.PrescriptionConditions)
                   .HasForeignKey(pc => pc.PrescriptionId)
                   .OnDelete(DeleteBehavior.Restrict);

            // من غير cascade: مينفعش تمسح مرض لسه مربوط بروشتة
            builder.HasOne(pc => pc.PatientCondition)
                   .WithMany(c => c.PrescriptionConditions)
                   .HasForeignKey(pc => pc.PatientConditionId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
