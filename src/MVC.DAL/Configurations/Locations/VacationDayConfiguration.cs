using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Locations;

namespace MVC.DAL.Configurations.Locations
{
    // VacationDays table
    public class VacationDayConfiguration : IEntityTypeConfiguration<VacationDay>
    {
        public void Configure(EntityTypeBuilder<VacationDay> builder)
        {
            builder.ToTable("VacationDays", t =>
            {
                // The vacation end date can't be before the start date (the range includes both ends)
                t.HasCheckConstraint("CK_VacationDays_DateRange", "[EndDate] >= [StartDate]");
            });

            builder.HasKey(v => v.VacationDayId);

            builder.HasOne(v => v.Doctor)
                   .WithMany(d => d.VacationDays)
                   .HasForeignKey(v => v.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
