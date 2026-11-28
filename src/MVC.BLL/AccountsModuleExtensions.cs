/*——- Omar Hussein ——-*/
using Microsoft.Extensions.DependencyInjection;
using MVC.BLL.Interfaces;
using MVC.BLL.Services.Accounts;
using MVC.BLL.Services.Infrastructure;

namespace MVC.BLL
{
    // تسجيل خدمات Module 1 (Accounts) في الـ DI - سطر واحد في Program.cs
    public static class AccountsModuleExtensions
    {
        public static IServiceCollection AddAccountsModule(this IServiceCollection services, string uploadsRootPath)
        {
            services.AddMemoryCache();

            // الملفات المحمية برة wwwroot
            services.AddSingleton(new FileStorageOptions { RootPath = uploadsRootPath });
            services.AddSingleton<IFileStorage, LocalFileStorage>();

            // Fake: الإيميل والـ SMS بيتكتبوا في الـ Log لحد ما يتربطوا بخدمة حقيقية
            services.AddSingleton<IEmailSender, LoggingEmailSender>();
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
/*————*/
