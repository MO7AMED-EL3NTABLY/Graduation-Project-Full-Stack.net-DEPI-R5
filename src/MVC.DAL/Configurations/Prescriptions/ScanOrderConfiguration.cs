using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Prescriptions;

namespace MVC.DAL.Configurations.Prescriptions
{
    // ScanOrders table
    public class ScanOrderConfiguration : IEntityTypeConfiguration<ScanOrder>
    {
        public void Configure(EntityTypeBuilder<ScanOrder> builder)
        {
            builder.ToTable("ScanOrders");

            builder.HasKey(s => s.ScanOrderId);

            builder.HasOne(s => s.Prescription)
                   .WithMany(p => p.ScanOrders)
                   .HasForeignKey(s => s.PrescriptionId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
