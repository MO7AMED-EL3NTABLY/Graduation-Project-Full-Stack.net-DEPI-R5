using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Chat;

namespace MVC.DAL.Configurations.Chat
{
    // ChatThreads table - unique on (PatientId, DoctorId)
    public class ChatThreadConfiguration : IEntityTypeConfiguration<ChatThread>
    {
        public void Configure(EntityTypeBuilder<ChatThread> builder)
        {
            builder.ToTable("ChatThreads");

            builder.HasKey(t => t.ChatThreadId);

            builder.HasOne(t => t.Patient)
                   .WithMany(p => p.ChatThreads)
                   .HasForeignKey(t => t.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Doctor)
                   .WithMany(d => d.ChatThreads)
                   .HasForeignKey(t => t.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(t => new { t.PatientId, t.DoctorId })
                   .IsUnique();
        }
    }
}
