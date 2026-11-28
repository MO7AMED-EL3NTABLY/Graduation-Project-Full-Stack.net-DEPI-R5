using Microsoft.Extensions.DependencyInjection;
using MVC.BLL.Interfaces;
using MVC.BLL.Services.Accounts;
using MVC.BLL.Services.Infrastructure;

namespace MVC.BLL
{
    // Registers the Module 1 (Accounts) services in DI - one line in Program.cs
    public static class AccountsModuleExtensions
    {
        public static IServiceCollection AddAccountsModule(this IServiceCollection services, string uploadsRootPath)
        {
            services.AddMemoryCache();

            // Protected files outside wwwroot
            services.AddSingleton(new FileStorageOptions { RootPath = uploadsRootPath });
            services.AddSingleton<IFileStorage, LocalFileStorage>();

            // Email sender is registered in Program.cs (SMTP, or the Fake one in Development only)
            // Fake: SMS is written to the log until it is connected to a real provider
            services.AddSingleton<ISmsSender, LoggingSmsSender>();

            services.AddScoped<IPhoneVerificationService, PhoneVerificationService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IPasswordResetService, PasswordResetService>();
            services.AddScoped<IAccountSettingsService, AccountSettingsService>();
            services.AddScoped<IDoctorProfileService, DoctorProfileService>();
            services.AddScoped<IDoctorApprovalService, DoctorApprovalService>();
            services.AddScoped<IProfileChangeRequestService, ProfileChangeRequestService>();
            services.AddScoped<IFileAccessService, FileAccessService>();

            return services;
        }
    }
}
