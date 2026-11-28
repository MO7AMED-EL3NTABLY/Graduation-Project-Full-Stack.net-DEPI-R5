using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace MVC.PL.Security
{
    // Password reset link is valid for 1 hour only (email confirmation link stays 24 hours from the default provider)
    public class PasswordResetTokenProviderOptions : DataProtectionTokenProviderOptions
    {
        public PasswordResetTokenProviderOptions()
        {
            Name = PasswordResetTokenProvider<object>.ProviderName;
            TokenLifespan = TimeSpan.FromHours(1);
        }
    }

    public class PasswordResetTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public const string ProviderName = "MediCarePasswordReset";

        public PasswordResetTokenProvider(
            IDataProtectionProvider dataProtectionProvider,
            IOptions<PasswordResetTokenProviderOptions> options,
            ILogger<DataProtectorTokenProvider<TUser>> logger)
            : base(dataProtectionProvider, options, logger)
        {
        }
    }
}
