using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Notifications;

namespace MVC.DAL.Configurations.Notifications
{
    // جدول Notifications
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications", t =>
            {
                // مصدر واحد بالكتير
                t.HasCheckConstraint("CK_Notifications_SingleSource",
                    "(CASE WHEN [AppointmentId] IS NOT NULL THEN 1 ELSE 0 END + " +
                    "CASE WHEN [ChatThreadId] IS NOT NULL THEN 1 ELSE 0 END + " +
                    "CASE WHEN [ScanOrderId] IS NOT NULL THEN 1 ELSE 0 END + " +
                    "CASE WHEN [PrescriptionId] IS NOT NULL THEN 1 ELSE 0 END) <= 1");
            });

            builder.HasKey(n => n.NotificationId);

            builder.HasOne(n => n.User)
                   .WithMany(u => u.Notifications)
                   .HasForeignKey(n => n.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(n => n.Appointment)
                   .WithMany(a => a.Notifications)
                   .HasForeignKey(n => n.AppointmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(n => n.ChatThread)
                   .WithMany(t => t.Notifications)
                   .HasForeignKey(n => n.ChatThreadId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(n => n.ScanOrder)
                   .WithMany(s => s.Notifications)
                   .HasForeignKey(n => n.ScanOrderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(n => n.Prescription)
                   .WithMany(p => p.Notifications)
                   .HasForeignKey(n => n.PrescriptionId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
