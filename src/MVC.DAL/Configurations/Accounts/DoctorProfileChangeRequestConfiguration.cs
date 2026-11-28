using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Configurations.Accounts
{
    // جدول DoctorProfileChangeRequests
    public class DoctorProfileChangeRequestConfiguration : IEntityTypeConfiguration<DoctorProfileChangeRequest>
    {
        public void Configure(EntityTypeBuilder<DoctorProfileChangeRequest> builder)
        {
            builder.ToTable("DoctorProfileChangeRequests", t =>
            {
                // العمود اللي يقابل RequestType بس هو اللي يتملى
                t.HasCheckConstraint("CK_DoctorProfileChangeRequests_RequestedValue",
                    "([RequestType] = 'Specialty' AND [RequestedSpecialtyId] IS NOT NULL AND [RequestedLicenseNumber] IS NULL AND [RequestedUniversity] IS NULL AND [RequestedGraduationYear] IS NULL) OR " +
                    "([RequestType] = 'LicenseNumber' AND [RequestedSpecialtyId] IS NULL AND [RequestedLicenseNumber] IS NOT NULL AND [RequestedUniversity] IS NULL AND [RequestedGraduationYear] IS NULL) OR " +
                    "([RequestType] = 'University' AND [RequestedSpecialtyId] IS NULL AND [RequestedLicenseNumber] IS NULL AND [RequestedUniversity] IS NOT NULL AND [RequestedGraduationYear] IS NULL) OR " +
                    "([RequestType] = 'GraduationYear' AND [RequestedSpecialtyId] IS NULL AND [RequestedLicenseNumber] IS NULL AND [RequestedUniversity] IS NULL AND [RequestedGraduationYear] IS NOT NULL)");

                // Pending: مفيش بيانات مراجعة / Approved أو Rejected: وقت المراجعة والأدمن الاتنين متملين
                t.HasCheckConstraint("CK_DoctorProfileChangeRequests_Review",
                    "([Status] = 'Pending' AND [ReviewedAtUtc] IS NULL AND [ReviewedByAdminId] IS NULL) OR " +
                    "([Status] IN ('Approved','Rejected') AND [ReviewedAtUtc] IS NOT NULL AND [ReviewedByAdminId] IS NOT NULL)");
            });

            builder.HasKey(r => r.DoctorProfileChangeRequestId);

            builder.HasOne(r => r.Doctor)
                   .WithMany(d => d.ChangeRequests)
                   .HasForeignKey(r => r.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.RequestedSpecialty)
                   .WithMany()
                   .HasForeignKey(r => r.RequestedSpecialtyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.ReviewedByAdmin)
                   .WithMany()
                   .HasForeignKey(r => r.ReviewedByAdminId)
                   .OnDelete(DeleteBehavior.Restrict);

            // طلب Pending واحد بالكتير لكل دكتور ونوع طلب
            builder.HasIndex(r => new { r.DoctorId, r.RequestType }, "UX_DoctorProfileChangeRequests_Pending")
                   .IsUnique()
                   .HasFilter("[Status] = 'Pending'");
        }
    }
}
