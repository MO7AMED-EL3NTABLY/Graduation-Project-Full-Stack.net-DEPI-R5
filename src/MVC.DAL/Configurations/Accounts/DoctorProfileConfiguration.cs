using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Configurations.Accounts
{
    // DoctorProfiles table - shared PK with AspNetUsers
    public class DoctorProfileConfiguration : IEntityTypeConfiguration<DoctorProfile>
    {
        public void Configure(EntityTypeBuilder<DoctorProfile> builder)
        {
            builder.ToTable("DoctorProfiles");

            builder.HasKey(d => d.DoctorId);

            // DoctorId is the PK and the FK to AspNetUsers.Id (1 : 0..1 relationship)
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
