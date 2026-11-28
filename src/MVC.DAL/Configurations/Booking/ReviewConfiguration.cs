using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Booking;

namespace MVC.DAL.Configurations.Booking
{
    // Reviews table
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews", t =>
            {
                // Rating from 1 to 5 (team decision: CHECK in the database + validation in the BLL)
                t.HasCheckConstraint("CK_Reviews_Rating", "[Rating] BETWEEN 1 AND 5");
            });

            builder.HasKey(r => r.ReviewId);

            builder.HasOne(r => r.Patient)
                   .WithMany(p => p.Reviews)
                   .HasForeignKey(r => r.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Doctor)
                   .WithMany(d => d.Reviews)
                   .HasForeignKey(r => r.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            // One non-deleted review per patient and doctor
            builder.HasIndex(r => new { r.PatientId, r.DoctorId }, "UX_Reviews_Patient_Doctor_NotDeleted")
                   .IsUnique()
                   .HasFilter("[DeletedAtUtc] IS NULL");
        }
    }
}
