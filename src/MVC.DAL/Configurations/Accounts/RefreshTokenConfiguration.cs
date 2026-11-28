using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Configurations.Accounts
{
    // RefreshTokens table
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(t => t.RefreshTokenId);

            builder.HasOne(t => t.User)
                   .WithMany(u => u.RefreshTokens)
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Self-reference: the token that replaced it (0..1 : 0..1)
            builder.HasOne(t => t.ReplacedByToken)
                   .WithOne()
                   .HasForeignKey<RefreshToken>(t => t.ReplacedByTokenId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Each token replaces at most one token
            builder.HasIndex(t => t.ReplacedByTokenId)
                   .IsUnique()
                   .HasFilter("[ReplacedByTokenId] IS NOT NULL");
        }
    }
}
