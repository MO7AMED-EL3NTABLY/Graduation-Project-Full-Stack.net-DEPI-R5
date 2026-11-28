/*——- Omar Hussein ——-*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MVC.BLL.Interfaces;
using MVC.DAL.Constants;
using MVC.DAL.Entities.Accounts;
using MVC.PL.Controllers;
using MVC.PL.Security;
using MVC.PL.ViewModels.Accounts;

namespace MVC.PL.Areas.Patient.Controllers
{
    // Settings المريض: البروفايل (الاسم والصورة) + الإيميل والموبايل والباسورد + اللغة
    [Area("Patient")]
    [Authorize(Roles = AppRoles.Patient)]
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

        protected override bool IsDoctor => false;

        [HttpPost]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public async Task<IActionResult> Profile([Bind(Prefix = "Profile")] ProfileFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await Settings.UpdatePatientProfileAsync(UserId, model.FullName, await model.Photo.ToFileUploadAsync());
                if (result.Succeeded)
                {
                    // الاسم في الـ Topbar جاي من الـ claims
                    var user = await Users.FindByIdAsync(UserId);
                    if (user is not null)
                        await SignInMgr.RefreshSignInAsync(user);
                    this.Flash(L["Your changes were saved."]);
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddErrors(result, L);
            }

            var page = await BuildPageAsync();
            if (page is null)
                return NotFound();
            page.Profile = model;
            return View(SettingsView, page);
        }
    }
}
/*————*/
