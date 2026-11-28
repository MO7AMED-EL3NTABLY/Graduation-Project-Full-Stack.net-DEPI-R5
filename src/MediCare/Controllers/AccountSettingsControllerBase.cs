/*——- Omar Hussein ——-*/
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MVC.BLL.Interfaces;
using MVC.DAL.Entities.Accounts;
using MVC.PL.Security;
using MVC.PL.ViewModels.Accounts;

namespace MVC.PL.Controllers
{
    // الجزء المشترك من صفحة Settings بين المريض والدكتور: الإيميل والموبايل والباسورد
    // (Patient/SettingsController و Doctor/SettingsController بيورثوا منه)
    [AutoValidateAntiforgeryToken]
    public abstract class AccountSettingsControllerBase : Controller
    {
        protected const string SettingsView = "~/Views/Shared/Accounts/Settings.cshtml";

        protected readonly IAccountSettingsService Settings;
        protected readonly IPhoneVerificationService Phone;
        protected readonly UserManager<ApplicationUser> Users;
        protected readonly SignInManager<ApplicationUser> SignInMgr;
        protected readonly IStringLocalizer<SharedResource> L;

        protected AccountSettingsControllerBase(
            IAccountSettingsService settings,
            IPhoneVerificationService phone,
            UserManager<ApplicationUser> users,
            SignInManager<ApplicationUser> signIn,
            IStringLocalizer<SharedResource> localizer)
        {
            Settings = settings;
            Phone = phone;
            Users = users;
            SignInMgr = signIn;
            L = localizer;
        }

        protected abstract bool IsDoctor { get; }

        protected string UserId => Users.GetUserId(User)!;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var page = await BuildPageAsync();
            return page is null ? NotFound() : View(SettingsView, page);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail([Bind(Prefix = "ChangeEmail")] ChangeEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await Settings.RequestEmailChangeAsync(UserId, model.NewEmail, EmailChangeLink);
                if (result.Succeeded)
                {
                    TempData["mc-email-sent"] = model.NewEmail.Trim();
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddErrors(result, L);
            }
            return await RedisplayAsync("email", p => p.ChangeEmail = model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePhone([Bind(Prefix = "ChangePhone")] ChangePhoneViewModel model)
        {
            if (ModelState.IsValid)
            {
                // الرقم الحالي بيفضل شغال لحد ما الكود يتأكد
                var result = await Settings.RequestPhoneChangeAsync(UserId, model.NewPhoneNumber);
                if (result.Succeeded)
                {
                    this.Flash(L["We sent a code to your new number."]);
                    TempData["mc-open-modal"] = "code";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddErrors(result, L);
            }
            return await RedisplayAsync("phone", p => p.ChangePhone = model);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyPhone([Bind(Prefix = "VerifyCode")] VerifyPhoneViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await Phone.VerifyCodeAsync(UserId, model.Code);
                if (result.Succeeded)
                {
                    this.Flash(L["Your phone number was updated."]);
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddErrors(result, L);
            }
            return await RedisplayAsync("code", p => p.VerifyCode = new VerifyPhoneViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ResendPhoneCode()
        {
            this.FlashResult(await Phone.ResendCodeAsync(UserId), "We sent you a new code.", L);
            TempData["mc-open-modal"] = "code";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([Bind(Prefix = "ChangePassword")] ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await Settings.ChangePasswordAsync(UserId, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    // نحدّث الـ cookie عشان الـ Security stamp اتغير
                    var user = await Users.FindByIdAsync(UserId);
                    if (user is not null)
                        await SignInMgr.RefreshSignInAsync(user);
                    this.Flash(L["Your password was changed."]);
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddErrors(result, L);
            }
            return await RedisplayAsync("password", p => p.ChangePassword = new ChangePasswordViewModel());
        }

        protected async Task<SettingsPageViewModel?> BuildPageAsync()
        {
            var account = await Settings.GetAsync(UserId);
            if (account is null)
                return null;

            var (pendingPhone, resendIn) = await Phone.GetActiveCodeInfoAsync(UserId);
            return new SettingsPageViewModel
            {
                Account = account,
                IsDoctor = IsDoctor,
                PendingPhone = pendingPhone is null ? null : MVC.BLL.Common.ContactFormat.FormatPhone(pendingPhone),
                ResendInSeconds = resendIn,
                Profile = new ProfileFormViewModel { FullName = account.FullName },
                OpenModal = TempData["mc-open-modal"] as string
            };
        }

        // بنرجّع نفس الصفحة بالأخطاء والمودال مفتوح
        protected async Task<IActionResult> RedisplayAsync(string modal, Action<SettingsPageViewModel> apply)
        {
            var page = await BuildPageAsync();
            if (page is null)
                return NotFound();
            page.OpenModal = modal;
            apply(page);
            return View(SettingsView, page);
        }

        private string EmailChangeLink(string userId, string newEmail, string token) =>
            Url.Action("ConfirmEmailChange", "Account", new { area = "", userId, email = newEmail, token }, Request.Scheme)!;
    }
}
/*————*/
