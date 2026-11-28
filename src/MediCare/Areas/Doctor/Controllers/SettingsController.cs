using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Mvc;
using MVC.BLL.Interfaces;
using MVC.DAL.Constants;
using MVC.DAL.Entities.Accounts;
using MVC.PL.Controllers;

namespace MVC.PL.Areas.Doctor.Controllers
{
    // Doctor settings: email, phone, password and language (professional data is in Professional profile)
    [Area("Doctor")]
    [Authorize(Roles = AppRoles.Doctor)]
    public class SettingsController : AccountSettingsControllerBase
    {
        public SettingsController(
            IAccountSettingsService settings,
            IPhoneVerificationService phone,
            UserManager<ApplicationUser> users,
            SignInManager<ApplicationUser> signIn,
            IStringLocalizer<SharedResource> localizer)
            : base(settings, phone, users, signIn, localizer)
        {
        }

        protected override bool IsDoctor => true;
    }
}
