using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Configurations.Accounts
{
    // PatientProfiles table - shared PK with AspNetUsers
    public class PatientProfileConfiguration : IEntityTypeConfiguration<PatientProfile>
    {
        public void Configure(EntityTypeBuilder<PatientProfile> builder)
        {
            builder.ToTable("PatientProfiles");

            builder.HasKey(p => p.PatientId);

            // PatientId is the PK and the FK to AspNetUsers.Id (1 : 0..1 relationship)
            builder.HasOne(p => p.User)
                   .WithOne(u => u.PatientProfile)
                   .HasForeignKey<PatientProfile>(p => p.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(p => p.ShareMedicalProfile)
                   .HasDefaultValue(false);
        }
    }
}
