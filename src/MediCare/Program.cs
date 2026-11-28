using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using MVC.BLL;
using MVC.BLL.Interfaces;
using MVC.BLL.Services.Infrastructure;
using MVC.DAL.Dbcontexts;
using MVC.DAL.Entities.Accounts;
using MVC.DAL.Seeding;
using MVC.PL.Security;
using System.Globalization;
using System.Net.Mail;

namespace MVC.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews()
                // Validation messages are translated from the same SharedResource file
                .AddDataAnnotationsLocalization(options =>
                    options.DataAnnotationLocalizerProvider = (_, factory) => factory.Create(typeof(SharedResource)));

            // Localization: Arabic texts live in Resources/SharedResource.ar.resx
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            // Database - MediCareDbContext on SQL Server
            builder.Services.AddDbContext<MediCareDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity - users and roles (Patient, Doctor, Admin)
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;                            // Email is required for every account
                options.Lockout.MaxFailedAccessAttempts = 5;                       // 5 failed sign-in attempts in a row
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // Lock the account for 15 minutes

                // Password rules from Figma: 8 characters + upper and lower case + a digit
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;

                // Password reset link is valid for 1 hour (email confirmation stays 24 hours)
                options.Tokens.PasswordResetTokenProvider = PasswordResetTokenProvider<ApplicationUser>.ProviderName;
            })
            .AddEntityFrameworkStores<MediCareDbContext>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<PasswordResetTokenProvider<ApplicationUser>>(PasswordResetTokenProvider<ApplicationUser>.ProviderName)
            .AddClaimsPrincipalFactory<AppClaimsPrincipalFactory>(); // Adds FullName to the claims

            // Login and access-denied pages (built in the Accounts module)
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            // Module 1 (Accounts): services + protected files outside wwwroot + SMS (Fake - written to the log)
            builder.Services.AddAccountsModule(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "uploads"));
            builder.Services.AddSingleton<AccountFlowProtector>();

            // Email:
            // - Smtp section exists (User Secrets / environment) -> real SMTP, and every setting is validated at startup
            // - No Smtp section + Development -> Fake sender that writes the message (and its link) to the log
            // - No Smtp section outside Development -> the app refuses to start (links must never go to the log)
            var smtpSection = builder.Configuration.GetSection("Smtp");
            if (smtpSection.Exists())
            {
                builder.Services.AddOptions<SmtpOptions>()
                    .Bind(smtpSection)
                    .Validate(o => !string.IsNullOrWhiteSpace(o.Host), "Smtp:Host is required.")
                    .Validate(o => o.Port is > 0 and <= 65535, "Smtp:Port must be between 1 and 65535.")
                    .Validate(o => MailAddress.TryCreate(o.Username, out _), "Smtp:Username must be a valid email address (it is used as the sender).")
                    .Validate(o => !string.IsNullOrWhiteSpace(o.Password), "Smtp:Password is required.")
                    .Validate(o => !string.IsNullOrWhiteSpace(o.FromName), "Smtp:FromName is required.")
                    .ValidateOnStart(); // A wrong setting stops the app at startup, not at the first registration
                builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();
            }
            else if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddSingleton<IEmailSender, LoggingEmailSender>();
            }
            else
            {
                throw new InvalidOperationException(
                    "The Smtp section is missing. The fake email sender (which logs links) is allowed only in Development.");
            }

            var app = builder.Build();

            // Seed: the three roles and the first Admin account
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var seedAdmin = builder.Configuration.GetSection("SeedAdmin");

                IdentitySeeder.SeedAsync(
                    services.GetRequiredService<MediCareDbContext>(),
                    services.GetRequiredService<UserManager<ApplicationUser>>(),
                    services.GetRequiredService<RoleManager<IdentityRole>>(),
                    seedAdmin["Email"],
                    seedAdmin["Password"],
                    seedAdmin["FullName"])
                    .GetAwaiter().GetResult();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Language: English (default) or Arabic - ar-EG so dates stay Gregorian
            var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("ar-EG") };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseRouting();

            app.UseAuthentication(); // Must come before UseAuthorization
            app.UseAuthorization();

            // The Areas route (Patient / Doctor / Admin) must come before the default route
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
