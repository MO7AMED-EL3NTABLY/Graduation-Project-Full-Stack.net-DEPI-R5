using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Support;

namespace MVC.DAL.Configurations.Support
{
    // SupportMessages table
    public class SupportMessageConfiguration : IEntityTypeConfiguration<SupportMessage>
    {
        public void Configure(EntityTypeBuilder<SupportMessage> builder)
        {
            builder.ToTable("SupportMessages");

            builder.HasKey(m => m.SupportMessageId);

            builder.HasOne(m => m.SupportConversation)
                   .WithMany(c => c.Messages)
                   .HasForeignKey(m => m.SupportConversationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.SenderUser)
                   .WithMany()
                   .HasForeignKey(m => m.SenderUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
