using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Notifications;

namespace MVC.DAL.Configurations.Notifications
{
    // جدول NotificationPreferences - Composite PK (UserId, EventType, Channel)
    public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
    {
        public void Configure(EntityTypeBuilder<NotificationPreference> builder)
        {
            builder.ToTable("NotificationPreferences");

            builder.HasKey(p => new { p.UserId, p.EventType, p.Channel });

            builder.HasOne(p => p.User)
                   .WithMany(u => u.NotificationPreferences)
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
