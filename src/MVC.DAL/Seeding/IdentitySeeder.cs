using Microsoft.AspNetCore.Identity;
using MVC.DAL.Constants;
using MVC.DAL.Dbcontexts;
using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Seeding
{
    // Adds the three roles and the first Admin account if they don't exist - runs once on every app start
    // Safe to repeat: never adds something that already exists
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(
            MediCareDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            string? adminEmail,
            string? adminPassword,
            string? adminFullName)
        {
            // 1) Roles: Patient, Doctor, Admin
            foreach (var role in AppRoles.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                    EnsureSucceeded(roleResult, $"create role '{role}'");
                }
            }

            // 2) The first Admin account (if its data exists in the configuration)
            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
                return;

            if (await userManager.FindByEmailAsync(adminEmail) is not null)
                return;

            // Create the account and the role in one transaction - both are saved or nothing is
            // The admin has no profile, and phone and photo are not required for it
            await using var transaction = await context.Database.BeginTransactionAsync();

            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = string.IsNullOrWhiteSpace(adminFullName) ? "System Admin" : adminFullName,
                IsActive = true
            };

            var createResult = await userManager.CreateAsync(admin, adminPassword);
            EnsureSucceeded(createResult, "create admin user");

            var roleAssignResult = await userManager.AddToRoleAsync(admin, AppRoles.Admin);
            EnsureSucceeded(roleAssignResult, "assign Admin role");

            await transaction.CommitAsync();
        }

        private static void EnsureSucceeded(IdentityResult result, string action)
        {
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Identity seeding failed to {action}: {errors}");
            }
        }
    }
}
