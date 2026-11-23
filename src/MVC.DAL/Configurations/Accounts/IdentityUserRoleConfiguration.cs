using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MVC.DAL.Configurations.Accounts
{
    // جدول AspNetUserRoles - Unique index على UserId عشان كل حساب ياخد role واحد بالكتير
    public class IdentityUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasIndex(ur => ur.UserId)
                   .IsUnique();
        }
    }
}
