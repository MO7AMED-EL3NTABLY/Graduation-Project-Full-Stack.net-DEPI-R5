using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Configurations.Accounts
{
    // جدول PatientProfiles - Shared PK مع AspNetUsers
    public class PatientProfileConfiguration : IEntityTypeConfiguration<PatientProfile>
    {
        public void Configure(EntityTypeBuilder<PatientProfile> builder)
        {
            builder.ToTable("PatientProfiles");

            builder.HasKey(p => p.PatientId);

            // PatientId هو PK و FK لـ AspNetUsers.Id (علاقة 1 : 0..1)
            builder.HasOne(p => p.User)
                   .WithOne(u => u.PatientProfile)
                   .HasForeignKey<PatientProfile>(p => p.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(p => p.ShareMedicalProfile)
                   .HasDefaultValue(false);
        }
    }
}
