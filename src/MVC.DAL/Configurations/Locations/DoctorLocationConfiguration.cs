using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Locations;

namespace MVC.DAL.Configurations.Locations
{
    // جدول DoctorLocations - LocationId عليه unique index
    public class DoctorLocationConfiguration : IEntityTypeConfiguration<DoctorLocation>
    {
        public void Configure(EntityTypeBuilder<DoctorLocation> builder)
        {
            builder.ToTable("DoctorLocations");

            builder.HasKey(dl => dl.DoctorLocationId);

            builder.HasOne(dl => dl.Doctor)
                   .WithMany(d => d.DoctorLocations)
                   .HasForeignKey(dl => dl.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            // كل Location ليه DoctorLocation واحد بالكتير (1 : 0..1)
            builder.HasOne(dl => dl.Location)
                   .WithOne(l => l.DoctorLocation)
                   .HasForeignKey<DoctorLocation>(dl => dl.LocationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(dl => dl.LocationId)
                   .IsUnique();
        }
    }
}
