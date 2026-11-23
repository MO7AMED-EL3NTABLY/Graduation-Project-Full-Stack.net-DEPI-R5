using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Configurations.Accounts
{
    // جدول PhoneVerificationCodes
    public class PhoneVerificationCodeConfiguration : IEntityTypeConfiguration<PhoneVerificationCode>
    {
        public void Configure(EntityTypeBuilder<PhoneVerificationCode> builder)
        {
            builder.ToTable("PhoneVerificationCodes");

            builder.HasKey(c => c.PhoneVerificationCodeId);

            builder.HasOne(c => c.User)
                   .WithMany(u => u.PhoneVerificationCodes)
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
