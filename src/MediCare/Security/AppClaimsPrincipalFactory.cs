using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MVC.DAL.Entities.Accounts;
using System.Security.Claims;

namespace MVC.PL.Security
{
    // بيضيف FullName للـ claims وقت تسجيل الدخول عشان يظهر في الـ Topbar من غير ما نروح للداتابيز كل request
    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public const string FullNameClaim = "FullName";

        public AppClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim(FullNameClaim, user.FullName));
            return identity;
        }
    }
}
