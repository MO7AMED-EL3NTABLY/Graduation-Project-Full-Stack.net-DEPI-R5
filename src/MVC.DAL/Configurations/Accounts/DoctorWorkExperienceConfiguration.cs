using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Configurations.Accounts
{
    // جدول DoctorWorkExperiences
    public class DoctorWorkExperienceConfiguration : IEntityTypeConfiguration<DoctorWorkExperience>
    {
        public void Configure(EntityTypeBuilder<DoctorWorkExperience> builder)
        {
            builder.ToTable("DoctorWorkExperiences");

            builder.HasKey(e => e.DoctorWorkExperienceId);

            builder.HasOne(e => e.Doctor)
                   .WithMany(d => d.WorkExperiences)
                   .HasForeignKey(e => e.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
