using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Chat;

namespace MVC.DAL.Configurations.Chat
{
    // ChatMessages table
    public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.ToTable("ChatMessages", t =>
            {
                // Text, a file, or both are required
                t.HasCheckConstraint("CK_ChatMessages_Content",
                    "[Text] IS NOT NULL OR [AttachmentStorageKey] IS NOT NULL");
            });

            builder.HasKey(m => m.ChatMessageId);

            builder.HasOne(m => m.ChatThread)
                   .WithMany(t => t.Messages)
                   .HasForeignKey(m => m.ChatThreadId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.SenderUser)
                   .WithMany()
                   .HasForeignKey(m => m.SenderUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
