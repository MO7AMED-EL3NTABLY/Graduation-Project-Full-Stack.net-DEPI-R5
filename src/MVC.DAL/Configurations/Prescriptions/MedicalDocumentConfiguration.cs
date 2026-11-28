using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Prescriptions;

namespace MVC.DAL.Configurations.Prescriptions
{
    // MedicalDocuments table
    public class MedicalDocumentConfiguration : IEntityTypeConfiguration<MedicalDocument>
    {
        public void Configure(EntityTypeBuilder<MedicalDocument> builder)
        {
            builder.ToTable("MedicalDocuments");

            builder.HasKey(d => d.MedicalDocumentId);

            builder.HasOne(d => d.Patient)
                   .WithMany(p => p.MedicalDocuments)
                   .HasForeignKey(d => d.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.ScanOrder)
                   .WithMany(s => s.MedicalDocuments)
                   .HasForeignKey(d => d.ScanOrderId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
