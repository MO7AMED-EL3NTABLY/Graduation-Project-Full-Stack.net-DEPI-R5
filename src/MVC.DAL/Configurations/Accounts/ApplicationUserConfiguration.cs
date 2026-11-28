using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Configurations.Accounts
{
    // AspNetUsers table - the extra columns on top of IdentityUser
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // The AspNetUsers table name is set by Identity itself
            builder.Property(u => u.FullName)
                   .IsRequired();
        }
    }
}
