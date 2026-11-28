using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Configurations.Accounts
{
    // DoctorDocuments table - has 3 CHECKs and 4 filtered unique indexes
    public class DoctorDocumentConfiguration : IEntityTypeConfiguration<DoctorDocument>
    {
        public void Configure(EntityTypeBuilder<DoctorDocument> builder)
        {
            builder.ToTable("DoctorDocuments", t =>
            {
                // Rejection reason is required when Rejected
                t.HasCheckConstraint("CK_DoctorDocuments_RejectionReason",
                    "[ApprovalStatus] <> 'Rejected' OR [RejectionReason] IS NOT NULL");

                // Review time and admin: both null while Pending, and both set when Approved or Rejected
                t.HasCheckConstraint("CK_DoctorDocuments_Review",
                    "([ApprovalStatus] = 'Pending' AND [ReviewedAtUtc] IS NULL AND [ReviewedByAdminId] IS NULL) OR " +
                    "([ApprovalStatus] IN ('Approved', 'Rejected') AND [ReviewedAtUtc] IS NOT NULL AND [ReviewedByAdminId] IS NOT NULL)");

                // IsCurrent = 1 only when Approved
                t.HasCheckConstraint("CK_DoctorDocuments_IsCurrent",
                    "[IsCurrent] = 0 OR [ApprovalStatus] = 'Approved'");
            });

            builder.HasKey(d => d.DoctorDocumentId);

            builder.HasOne(d => d.Doctor)
                   .WithMany(p => p.Documents)
                   .HasForeignKey(d => d.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.ReviewedByAdmin)
                   .WithMany()
                   .HasForeignKey(d => d.ReviewedByAdminId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Self-reference: the old document this one replaces
            builder.HasOne(d => d.ReplacesDocument)
                   .WithMany()
                   .HasForeignKey(d => d.ReplacesDocumentId)
                   .OnDelete(DeleteBehavior.Restrict);

            // At most one current approved license per doctor
            builder.HasIndex(d => d.DoctorId, "UX_DoctorDocuments_License_Approved_Current")
                   .IsUnique()
                   .HasFilter("[DocumentType] = 'License' AND [ApprovalStatus] = 'Approved' AND [IsCurrent] = 1");

            // At most one current approved graduation certificate per doctor
            builder.HasIndex(d => d.DoctorId, "UX_DoctorDocuments_GraduationCertificate_Approved_Current")
                   .IsUnique()
                   .HasFilter("[DocumentType] = 'GraduationCertificate' AND [ApprovalStatus] = 'Approved' AND [IsCurrent] = 1");

            // At most one Pending license per doctor
            builder.HasIndex(d => d.DoctorId, "UX_DoctorDocuments_License_Pending")
                   .IsUnique()
                   .HasFilter("[DocumentType] = 'License' AND [ApprovalStatus] = 'Pending'");

            // At most one Pending graduation certificate per doctor
            builder.HasIndex(d => d.DoctorId, "UX_DoctorDocuments_GraduationCertificate_Pending")
                   .IsUnique()
                   .HasFilter("[DocumentType] = 'GraduationCertificate' AND [ApprovalStatus] = 'Pending'");
        }
    }
}
