using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using MVC.DAL.Dbcontexts;
using MVC.DAL.Entities.Accounts;
using MVC.DAL.Seeding;
using MVC.PL.Security;
using System.Globalization;

namespace MVC.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // الترجمة: النصوص العربية في Resources/SharedResource.ar.resx
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            // الداتابيز - MediCareDbContext على SQL Server
            builder.Services.AddDbContext<MediCareDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity - المستخدمين والـ Roles (Patient, Doctor, Admin)
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;                            // الإيميل مطلوب لكل حساب
                options.Lockout.MaxFailedAccessAttempts = 5;                       // 5 محاولات دخول غلط ورا بعض
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // قفل الحساب 15 دقيقة
            })
            .AddEntityFrameworkStores<MediCareDbContext>()
            .AddDefaultTokenProviders()
            .AddClaimsPrincipalFactory<AppClaimsPrincipalFactory>(); // بيضيف FullName للـ claims

            // صفحات الدخول ومنع الوصول (هتتعمل في Accounts module)
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            var app = builder.Build();

            // Seed: الـ Roles الثلاثة وأول حساب Admin
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

            // اللغة: إنجليزي (افتراضي) أو عربي - ar-EG عشان التواريخ تبقى ميلادي
            var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("ar-EG") };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseRouting();

            app.UseAuthentication(); // لازم قبل UseAuthorization
            app.UseAuthorization();

            // Route الـ Areas (Patient / Doctor / Admin) لازم يكون قبل الـ default
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
