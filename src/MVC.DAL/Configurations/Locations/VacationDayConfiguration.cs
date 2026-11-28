using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Locations;

namespace MVC.DAL.Configurations.Locations
{
    // جدول VacationDays
    public class VacationDayConfiguration : IEntityTypeConfiguration<VacationDay>
    {
        public void Configure(EntityTypeBuilder<VacationDay> builder)
        {
            builder.ToTable("VacationDays", t =>
            {
                // تاريخ نهاية الإجازة مينفعش يكون قبل تاريخ البداية (الفترة شاملة الطرفين)
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
