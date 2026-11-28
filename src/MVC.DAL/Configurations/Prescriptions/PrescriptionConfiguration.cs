using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Prescriptions;

namespace MVC.DAL.Configurations.Prescriptions
{
    // Prescriptions table
    public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            builder.ToTable("Prescriptions");

            builder.HasKey(p => p.PrescriptionId);

            // The original appointment the prescription was created in
            builder.HasOne(p => p.Appointment)
                   .WithMany(a => a.Prescriptions)
                   .HasForeignKey(p => p.AppointmentId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
