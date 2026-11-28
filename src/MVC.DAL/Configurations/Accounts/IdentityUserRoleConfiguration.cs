using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MVC.DAL.Configurations.Accounts
{
    // AspNetUserRoles table - unique index on UserId so each account gets at most one role
    public class IdentityUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasIndex(ur => ur.UserId)
                   .IsUnique();
        }
    }
}
