using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Prescriptions;

namespace MVC.DAL.Configurations.Prescriptions
{
    // جدول PatientConditions
    public class PatientConditionConfiguration : IEntityTypeConfiguration<PatientCondition>
    {
        public void Configure(EntityTypeBuilder<PatientCondition> builder)
        {
            builder.ToTable("PatientConditions");

            builder.HasKey(c => c.PatientConditionId);

            builder.HasOne(c => c.Patient)
                   .WithMany(p => p.Conditions)
                   .HasForeignKey(c => c.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            // الدكتور اللي سجل المرض ومسؤول عنه (NOT NULL)
            builder.HasOne(c => c.CreatedByDoctor)
                   .WithMany(d => d.CreatedConditions)
                   .HasForeignKey(c => c.CreatedByDoctorId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
