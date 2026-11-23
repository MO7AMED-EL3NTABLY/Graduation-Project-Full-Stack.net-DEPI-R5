using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Configurations.Accounts
{
    // جدول DoctorSurgeries
    public class DoctorSurgeryConfiguration : IEntityTypeConfiguration<DoctorSurgery>
    {
        public void Configure(EntityTypeBuilder<DoctorSurgery> builder)
        {
            builder.ToTable("DoctorSurgeries");

            builder.HasKey(s => s.DoctorSurgeryId);

            builder.HasOne(s => s.Doctor)
                   .WithMany(d => d.Surgeries)
                   .HasForeignKey(s => s.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
