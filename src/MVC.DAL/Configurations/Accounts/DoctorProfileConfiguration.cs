using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Configurations.Accounts
{
    // جدول DoctorProfiles - Shared PK مع AspNetUsers
    public class DoctorProfileConfiguration : IEntityTypeConfiguration<DoctorProfile>
    {
        public void Configure(EntityTypeBuilder<DoctorProfile> builder)
        {
            builder.ToTable("DoctorProfiles");

            builder.HasKey(d => d.DoctorId);

            // DoctorId هو PK و FK لـ AspNetUsers.Id (علاقة 1 : 0..1)
            builder.HasOne(d => d.User)
                   .WithOne(u => u.DoctorProfile)
                   .HasForeignKey<DoctorProfile>(d => d.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Specialty)
                   .WithMany(s => s.Doctors)
                   .HasForeignKey(d => d.SpecialtyId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
