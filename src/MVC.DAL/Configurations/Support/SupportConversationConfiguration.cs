using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Support;

namespace MVC.DAL.Configurations.Support
{
    // جدول SupportConversations - Unique على UserId
    public class SupportConversationConfiguration : IEntityTypeConfiguration<SupportConversation>
    {
        public void Configure(EntityTypeBuilder<SupportConversation> builder)
        {
            builder.ToTable("SupportConversations");

            builder.HasKey(c => c.SupportConversationId);

            // محادثة واحدة لكل حساب (1 : 0..1)
            builder.HasOne(c => c.User)
                   .WithOne(u => u.SupportConversation)
                   .HasForeignKey<SupportConversation>(c => c.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.UserId)
                   .IsUnique();
        }
    }
}
