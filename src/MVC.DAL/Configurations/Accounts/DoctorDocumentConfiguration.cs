using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Configurations.Accounts
{
    // جدول DoctorDocuments - فيه 3 CHECKs و 4 filtered unique indexes
    public class DoctorDocumentConfiguration : IEntityTypeConfiguration<DoctorDocument>
    {
        public void Configure(EntityTypeBuilder<DoctorDocument> builder)
        {
            builder.ToTable("DoctorDocuments", t =>
            {
                // سبب الرفض مطلوب لو Rejected
                t.HasCheckConstraint("CK_DoctorDocuments_RejectionReason",
                    "[ApprovalStatus] <> 'Rejected' OR [RejectionReason] IS NOT NULL");

                // وقت المراجعة والأدمن: الاتنين null وهو Pending، والاتنين ليهم قيمة لو Approved أو Rejected
                t.HasCheckConstraint("CK_DoctorDocuments_Review",
                    "([ApprovalStatus] = 'Pending' AND [ReviewedAtUtc] IS NULL AND [ReviewedByAdminId] IS NULL) OR " +
                    "([ApprovalStatus] IN ('Approved', 'Rejected') AND [ReviewedAtUtc] IS NOT NULL AND [ReviewedByAdminId] IS NOT NULL)");

                // IsCurrent = 1 لما يكون Approved بس
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

            // Self-reference: المستند القديم اللي ده بيستبدله
            builder.HasOne(d => d.ReplacesDocument)
                   .WithMany()
                   .HasForeignKey(d => d.ReplacesDocumentId)
                   .OnDelete(DeleteBehavior.Restrict);

            // رخصة واحدة معتمدة حالية بالكتير لكل دكتور
            builder.HasIndex(d => d.DoctorId, "UX_DoctorDocuments_License_Approved_Current")
                   .IsUnique()
                   .HasFilter("[DocumentType] = 'License' AND [ApprovalStatus] = 'Approved' AND [IsCurrent] = 1");

            // شهادة تخرج واحدة معتمدة حالية بالكتير لكل دكتور
            builder.HasIndex(d => d.DoctorId, "UX_DoctorDocuments_GraduationCertificate_Approved_Current")
                   .IsUnique()
                   .HasFilter("[DocumentType] = 'GraduationCertificate' AND [ApprovalStatus] = 'Approved' AND [IsCurrent] = 1");

            // رخصة واحدة Pending بالكتير لكل دكتور
            builder.HasIndex(d => d.DoctorId, "UX_DoctorDocuments_License_Pending")
                   .IsUnique()
                   .HasFilter("[DocumentType] = 'License' AND [ApprovalStatus] = 'Pending'");

            // شهادة تخرج واحدة Pending بالكتير لكل دكتور
            builder.HasIndex(d => d.DoctorId, "UX_DoctorDocuments_GraduationCertificate_Pending")
                   .IsUnique()
                   .HasFilter("[DocumentType] = 'GraduationCertificate' AND [ApprovalStatus] = 'Pending'");
        }
    }
}
