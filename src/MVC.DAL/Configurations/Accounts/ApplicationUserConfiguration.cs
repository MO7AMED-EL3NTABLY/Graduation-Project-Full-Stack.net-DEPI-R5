using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Configurations.Accounts
{
    // جدول AspNetUsers - الأعمدة الزيادة على IdentityUser
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // اسم الجدول AspNetUsers بيتحدد من Identity نفسه
            builder.Property(u => u.FullName)
                   .IsRequired();
        }
    }
}
