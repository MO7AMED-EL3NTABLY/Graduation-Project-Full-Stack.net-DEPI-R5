using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Prescriptions;

namespace MVC.DAL.Configurations.Prescriptions
{
    // PatientConditions table
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

            // The doctor who recorded the condition and is responsible for it (NOT NULL)
            builder.HasOne(c => c.CreatedByDoctor)
                   .WithMany(d => d.CreatedConditions)
                   .HasForeignKey(c => c.CreatedByDoctorId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
