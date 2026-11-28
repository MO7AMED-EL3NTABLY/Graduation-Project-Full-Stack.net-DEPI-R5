using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MVC.BLL.DTOs.Accounts;
using MVC.BLL.Interfaces;
using MVC.DAL.Constants;
using MVC.DAL.Entities.Accounts;
using MVC.PL.Security;
using MVC.PL.ViewModels.Accounts;

namespace MVC.PL.Controllers
{
    // One login page for everyone + registration + email and phone verification + password recovery + doctor approval status
    // All business rules live in the BLL; the controller signs the user in and decides where to send them
    [AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        private const string ResetTicketKey = "mc-reset-ticket";
        private const string ResetUserKey = "mc-reset-user";
        private const string ResetTokenKey = "mc-reset-token";

        private readonly IAccountService _accounts;
        private readonly IPhoneVerificationService _phone;
        private readonly IPasswordResetService _reset;
        private readonly IAccountSettingsService _settings;
        private readonly SignInManager<ApplicationUser> _signIn;
        private readonly UserManager<ApplicationUser> _users;
        private readonly AccountFlowProtector _flow;
        private readonly IStringLocalizer<SharedResource> L;

        public AccountController(
            IAccountService accounts,
            IPhoneVerificationService phone,
            IPasswordResetService reset,
            IAccountSettingsService settings,
            SignInManager<ApplicationUser> signIn,
            UserManager<ApplicationUser> users,
            AccountFlowProtector flow,
            IStringLocalizer<SharedResource> localizer)
        {
            _accounts = accounts;
            _phone = phone;
            _reset = reset;
            _settings = settings;
            _signIn = signIn;
            _users = users;
            _flow = flow;
            L = localizer;
        }

        // ================= Login / Logout =================

        [HttpGet]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return await RedirectSignedInUserAsync();
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = await _accounts.FindUserIdByEmailAsync(model.Email);
            var user = userId is null ? null : await _users.FindByIdAsync(userId);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, L["Incorrect email or password."]);
                return View(model);
            }

            // Counts failed attempts: 5 attempts = 15-minute lockout
            var check = await _signIn.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);
            if (check.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, L["Too many failed attempts. Your account is locked for 15 minutes."]);
                return View(model);
            }
            if (!check.Succeeded)
            {
                ModelState.AddModelError(string.Empty, L["Incorrect email or password."]);
                return View(model);
            }

            var state = await _accounts.GetLoginStateAsync(user.Id);
            if (state == LoginState.Inactive)
            {
                ModelState.AddModelError(string.Empty, L["This account has been deactivated. Please contact support."]);
                return View(model);
            }

            // Not verified yet: don't sign in, continue verification with a protected cookie
            if (state == LoginState.NeedsVerification)
            {
                _flow.SetVerifyCookie(Response, user.Id);
                return RedirectToAction(nameof(VerifyPhone));
            }

            await _signIn.SignInAsync(user, isPersistent: model.RememberMe);
            return RedirectForState(state, model.ReturnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signIn.SignOutAsync();
            _flow.ClearVerifyCookie(Response);
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

        // ================= Register =================

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return await RedirectSignedInUserAsync();
            return View();
        }

        [HttpPost]
        public IActionResult Register(string role) =>
            role == AppRoles.Doctor
                ? RedirectToAction(nameof(RegisterDoctor))
                : RedirectToAction(nameof(RegisterPatient));

        [HttpGet]
        public async Task<IActionResult> RegisterPatient()
        {
            if (User.Identity?.IsAuthenticated == true)
                return await RedirectSignedInUserAsync();
            return View(new RegisterPatientViewModel());
        }

        [HttpPost]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public async Task<IActionResult> RegisterPatient(RegisterPatientViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accounts.RegisterPatientAsync(new RegisterPatientDto
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.DateOfBirth!.Value,
                Gender = model.Gender!.Value,
                Password = model.Password,
                Photo = await model.Photo.ToFileUploadAsync()
            }, EmailConfirmationLink);

            if (!result.Succeeded)
            {
                ModelState.AddErrors(result, L);
                return View(model);
            }

            _flow.SetVerifyCookie(Response, result.Value!);
            return RedirectToAction(nameof(VerifyPhone));
        }

        [HttpGet]
        public async Task<IActionResult> RegisterDoctor()
        {
            if (User.Identity?.IsAuthenticated == true)
                return await RedirectSignedInUserAsync();
            return View(new RegisterDoctorViewModel { Specialties = await _accounts.GetActiveSpecialtiesAsync() });
        }

        [HttpPost]
        [RequestSizeLimit(80 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 80 * 1024 * 1024)]
        public async Task<IActionResult> RegisterDoctor(RegisterDoctorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Specialties = await _accounts.GetActiveSpecialtiesAsync();
                return View(model);
            }

            var result = await _accounts.RegisterDoctorAsync(new RegisterDoctorDto
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Password = model.Password,
                Photo = await model.Photo.ToFileUploadAsync(),
                SpecialtyId = model.SpecialtyId!.Value,
                LicenseNumber = model.LicenseNumber,
                GraduationYear = model.GraduationYear!.Value,
                University = model.University,
                PracticeStartYear = model.PracticeStartYear!.Value,
                Bio = model.Bio,
                License = await model.License.ToFileUploadAsync(),
                GraduationCertificate = await model.GraduationCertificate.ToFileUploadAsync()
            }, EmailConfirmationLink);

            if (!result.Succeeded)
            {
                ModelState.AddErrors(result, L);
                model.Specialties = await _accounts.GetActiveSpecialtiesAsync();
                return View(model);
            }

            _flow.SetVerifyCookie(Response, result.Value!);
            return RedirectToAction(nameof(VerifyPhone));
        }

        // ================= Verify phone & email =================

        [HttpGet]
        public async Task<IActionResult> VerifyPhone()
        {
            var status = await GetVerifyStatusAsync();
            if (status is null)
                return RedirectToAction(nameof(Login));
            if (status.IsComplete)
                return await CompleteVerificationAsync(status.UserId);
            if (!status.PhoneRequired || status.PhoneConfirmed)
                return RedirectToAction(nameof(VerifyEmail));

            // First time after login with no active code: send a code to the registered number
            if (!status.HasActiveCode)
            {
                await _phone.ResendCodeAsync(status.UserId);
                status = await _accounts.GetVerificationStatusAsync(status.UserId);
            }

            return View(new VerifyPhoneViewModel { Status = status });
        }

        [HttpPost]
        public async Task<IActionResult> VerifyPhone(VerifyPhoneViewModel model)
        {
            var userId = _flow.GetVerifyUserId(Request);
            if (userId is null)
                return RedirectToAction(nameof(Login));

            if (ModelState.IsValid)
            {
                var result = await _phone.VerifyCodeAsync(userId, model.Code);
                if (result.Succeeded)
                {
                    var status = await _accounts.GetVerificationStatusAsync(userId);
                    return status?.IsComplete == true
                        ? await CompleteVerificationAsync(userId)
                        : RedirectToAction(nameof(VerifyEmail));
                }
                ModelState.AddErrors(result, L);
            }

            model.Code = string.Empty;
            model.Status = await _accounts.GetVerificationStatusAsync(userId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResendPhoneCode()
        {
            var userId = _flow.GetVerifyUserId(Request);
            if (userId is null)
                return RedirectToAction(nameof(Login));

            this.FlashResult(await _phone.ResendCodeAsync(userId), "We sent you a new code.", L);
            return RedirectToAction(nameof(VerifyPhone));
        }

        [HttpGet]
        public IActionResult ChangeNumber()
        {
            if (_flow.GetVerifyUserId(Request) is null)
                return RedirectToAction(nameof(Login));
            return View(new ChangeNumberViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ChangeNumber(ChangeNumberViewModel model)
        {
            var userId = _flow.GetVerifyUserId(Request);
            if (userId is null)
                return RedirectToAction(nameof(Login));
            if (!ModelState.IsValid)
                return View(model);

            // The number changes only after the new code is verified
            var result = await _phone.SendCodeAsync(userId, model.PhoneNumber);
            if (!result.Succeeded)
            {
                ModelState.AddErrors(result, L);
                return View(model);
            }

            this.Flash(L["We sent a code to your new number."]);
            return RedirectToAction(nameof(VerifyPhone));
        }

        [HttpGet]
        public async Task<IActionResult> VerifyEmail()
        {
            var status = await GetVerifyStatusAsync();
            if (status is null)
                return RedirectToAction(nameof(Login));
            if (status.IsComplete)
                return await CompleteVerificationAsync(status.UserId);
            if (status.EmailConfirmed)
                return RedirectToAction(nameof(VerifyPhone));

            return View(status);
        }

        [HttpPost]
        public async Task<IActionResult> ResendEmail()
        {
            var userId = _flow.GetVerifyUserId(Request);
            if (userId is null)
                return RedirectToAction(nameof(Login));

            this.FlashResult(await _accounts.SendEmailConfirmationAsync(userId, EmailConfirmationLink), "We sent you a new confirmation link.", L);
            return RedirectToAction(nameof(VerifyEmail));
        }

        // The link from the email
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var result = await _accounts.ConfirmEmailAsync(userId, token);
            if (!result.Succeeded)
                return MessageView("circle-alert", "Link expired", L[result.Errors[0]], "Back to log in", Url.Action(nameof(Login)));

            // If it's the same browser that is verifying: continue right away
            if (_flow.GetVerifyUserId(Request) == userId)
            {
                var status = await _accounts.GetVerificationStatusAsync(userId);
                return status?.IsComplete == true
                    ? await CompleteVerificationAsync(userId)
                    : RedirectToAction(nameof(VerifyPhone));
            }

            return MessageView("circle-check", "Email confirmed", L["Your email is confirmed. You can log in now."], "Log in", Url.Action(nameof(Login)));
        }

        // Confirmation link for the new email from the Settings page
        [HttpGet]
        public async Task<IActionResult> ConfirmEmailChange(string userId, string email, string token)
        {
            var result = await _settings.ConfirmEmailChangeAsync(userId, email, token);
            if (!result.Succeeded)
                return MessageView("circle-alert", "Link expired", L[result.Errors[0]], "Back to log in", Url.Action(nameof(Login)));

            // The cookie holds the old email - refresh it if it's the same user
            if (_users.GetUserId(User) == userId)
            {
                var user = await _users.FindByIdAsync(userId);
                if (user is not null)
                    await _signIn.RefreshSignInAsync(user);
            }

            return MessageView("circle-check", "Email updated", L["Your new email is confirmed. Use it the next time you log in."],
                User.Identity?.IsAuthenticated == true ? "Back to settings" : "Log in",
                User.Identity?.IsAuthenticated == true ? SettingsUrl() : Url.Action(nameof(Login)));
        }

        // ================= Doctor approval status =================

        [HttpGet]
        [Authorize(Roles = AppRoles.Doctor)]
        public async Task<IActionResult> Pending()
        {
            var status = await _accounts.GetDoctorStatusAsync(_users.GetUserId(User)!);
            if (status is null)
                return RedirectToAction(nameof(Login));
            if (status.ApprovalStatus == MVC.DAL.Enums.ApprovalStatus.Approved)
                return Redirect("/Doctor");
            if (status.ApprovalStatus == MVC.DAL.Enums.ApprovalStatus.Rejected)
                return RedirectToAction(nameof(Rejected));
            return View(status);
        }

        [HttpGet]
        [Authorize(Roles = AppRoles.Doctor)]
        public async Task<IActionResult> Rejected()
        {
            var status = await _accounts.GetDoctorStatusAsync(_users.GetUserId(User)!);
            if (status is null)
                return RedirectToAction(nameof(Login));
            if (status.ApprovalStatus != MVC.DAL.Enums.ApprovalStatus.Rejected)
                return RedirectToAction(nameof(Pending));
            return View(new ResubmitDocumentsViewModel { Status = status });
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.Doctor)]
        [RequestSizeLimit(25 * 1024 * 1024)]
        public async Task<IActionResult> Rejected(ResubmitDocumentsViewModel model)
        {
            var doctorId = _users.GetUserId(User)!;
            if (ModelState.IsValid)
            {
                var result = await _accounts.ResubmitDoctorDocumentsAsync(doctorId,
                    await model.License.ToFileUploadAsync(),
                    await model.GraduationCertificate.ToFileUploadAsync());
                if (result.Succeeded)
                {
                    this.Flash(L["Your documents were sent for review."]);
                    return RedirectToAction(nameof(Pending));
                }
                ModelState.AddErrors(result, L);
            }

            model.Status = await _accounts.GetDoctorStatusAsync(doctorId);
            return View(model);
        }

        // ================= Forgot / reset password =================

        [HttpGet]
        public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var account = await _reset.FindAccountAsync(model.Email);
            if (account is null)
            {
                ModelState.AddModelError(string.Empty, L["We couldn't find an account with these details."]);
                return View(model);
            }

            return View("ResetOptions", new ResetOptionsViewModel
            {
                Ticket = _flow.CreateResetTicket(account.UserId),
                Account = account
            });
        }

        [HttpPost]
        public async Task<IActionResult> SendReset(ResetOptionsViewModel model)
        {
            var userId = _flow.ReadResetTicket(model.Ticket);
            if (userId is null)
                return ResetExpired();

            if (model.Method == ResetMethod.Sms)
            {
                var sms = await _reset.SendSmsCodeAsync(userId);
                if (!sms.Succeeded)
                {
                    ModelState.AddErrors(sms, L);
                    var account = await FindAccountByIdAsync(userId);
                    if (account is null)
                        return ResetExpired();
                    model.Account = account;
                    return View("ResetOptions", model);
                }

                TempData[ResetTicketKey] = model.Ticket;
                return RedirectToAction(nameof(ResetCode));
            }

            var email = await _reset.SendEmailLinkAsync(userId, PasswordResetLink);
            if (!email.Succeeded)
            {
                // Same handling as the SMS branch: stay on the options page and show the error
                ModelState.AddErrors(email, L);
                var account = await FindAccountByIdAsync(userId);
                if (account is null)
                    return ResetExpired();
                model.Account = account;
                return View("ResetOptions", model);
            }

            return MessageView("mail", "Check your email", L["We sent you a link to reset your password. It's valid for 1 hour."], "Back to log in", Url.Action(nameof(Login)));
        }

        [HttpGet]
        public async Task<IActionResult> ResetCode()
        {
            var ticket = TempData.Peek(ResetTicketKey) as string;
            var userId = _flow.ReadResetTicket(ticket);
            if (userId is null)
                return ResetExpired();

            var account = await FindAccountByIdAsync(userId);
            return View(new ResetCodeViewModel { Ticket = ticket!, MaskedPhone = account?.MaskedPhone ?? string.Empty });
        }

        [HttpPost]
        public async Task<IActionResult> ResetCode(ResetCodeViewModel model)
        {
            var userId = _flow.ReadResetTicket(model.Ticket);
            if (userId is null)
                return ResetExpired();

            if (ModelState.IsValid)
            {
                var result = await _reset.VerifySmsCodeAsync(userId, model.Code);
                if (result.Succeeded)
                {
                    TempData.Remove(ResetTicketKey);
                    TempData[ResetUserKey] = userId;
                    TempData[ResetTokenKey] = result.Value;
                    return RedirectToAction(nameof(ResetPassword));
                }
                ModelState.AddErrors(result, L);
            }

            TempData[ResetTicketKey] = model.Ticket;
            model.Code = string.Empty;
            model.MaskedPhone = (await FindAccountByIdAsync(userId))?.MaskedPhone ?? string.Empty;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResendResetCode(string ticket)
        {
            var userId = _flow.ReadResetTicket(ticket);
            if (userId is null)
                return ResetExpired();

            this.FlashResult(await _reset.SendSmsCodeAsync(userId), "We sent you a new code.", L);
            TempData[ResetTicketKey] = ticket;
            return RedirectToAction(nameof(ResetCode));
        }

        // From the email link (userId + token in the query) or after the SMS code (from TempData)
        [HttpGet]
        public IActionResult ResetPassword(string? userId = null, string? token = null)
        {
            userId ??= TempData[ResetUserKey] as string;
            token ??= TempData[ResetTokenKey] as string;
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return ResetExpired();

            return View(new ResetPasswordViewModel { UserId = userId, Token = token });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _reset.ResetPasswordAsync(model.UserId, model.Token, model.NewPassword);
            if (!result.Succeeded)
            {
                ModelState.AddErrors(result, L);
                return View(model);
            }

            // If it was locked out by failed attempts, unlock it
            var user = await _users.FindByIdAsync(model.UserId);
            if (user is not null)
            {
                await _users.SetLockoutEndDateAsync(user, null);
                await _users.ResetAccessFailedCountAsync(user);
            }

            return MessageView("circle-check", "Password updated", L["Your password was changed. You can log in with the new one."], "Log in", Url.Action(nameof(Login)));
        }

        // ================= Helpers =================

        private string EmailConfirmationLink(string userId, string token) =>
            Url.Action(nameof(ConfirmEmail), "Account", new { area = "", userId, token }, Request.Scheme)!;

        private string PasswordResetLink(string userId, string token) =>
            Url.Action(nameof(ResetPassword), "Account", new { area = "", userId, token }, Request.Scheme)!;

        private async Task<VerificationStatusDto?> GetVerifyStatusAsync()
        {
            var userId = _flow.GetVerifyUserId(Request);
            return userId is null ? null : await _accounts.GetVerificationStatusAsync(userId);
        }

        private async Task<ResetAccountDto?> FindAccountByIdAsync(string userId)
        {
            var user = await _users.FindByIdAsync(userId);
            return user?.Email is null ? null : await _reset.FindAccountAsync(user.Email);
        }

        // Verification finished: sign in and send the user to their area
        private async Task<IActionResult> CompleteVerificationAsync(string userId)
        {
            _flow.ClearVerifyCookie(Response);
            var user = await _users.FindByIdAsync(userId);
            if (user is null)
                return RedirectToAction(nameof(Login));

            var state = await _accounts.GetLoginStateAsync(userId);
            if (state is LoginState.Inactive or LoginState.NeedsVerification)
                return RedirectToAction(nameof(Login));

            await _signIn.SignInAsync(user, isPersistent: false);
            return RedirectForState(state, null);
        }

        private async Task<IActionResult> RedirectSignedInUserAsync()
        {
            var userId = _users.GetUserId(User);
            var state = userId is null ? LoginState.Inactive : await _accounts.GetLoginStateAsync(userId);
            if (state is LoginState.Inactive or LoginState.NeedsVerification)
            {
                await _signIn.SignOutAsync();
                return RedirectToAction(nameof(Login));
            }
            return RedirectForState(state, null);
        }

        private IActionResult RedirectForState(LoginState state, string? returnUrl)
        {
            // returnUrl only for accounts that can enter the dashboard
            var canUseReturnUrl = state is LoginState.Patient or LoginState.DoctorApproved or LoginState.Admin;
            if (canUseReturnUrl && !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return state switch
            {
                LoginState.Patient => Redirect("/Patient"),
                LoginState.DoctorApproved => Redirect("/Doctor"),
                LoginState.DoctorPending => RedirectToAction(nameof(Pending)),
                LoginState.DoctorRejected => RedirectToAction(nameof(Rejected)),
                LoginState.Admin => Redirect("/Admin"),
                _ => RedirectToAction(nameof(Login))
            };
        }

        private string SettingsUrl() => User.IsInRole(AppRoles.Doctor) ? "/Doctor/Settings" : "/Patient/Settings";

        private IActionResult ResetExpired()
        {
            this.Flash(L["Your reset session has expired. Please start again."], success: false);
            return RedirectToAction(nameof(ForgotPassword));
        }

        private ViewResult MessageView(string icon, string title, string text, string? actionText, string? actionUrl) =>
            View("Message", new AccountMessageViewModel
            {
                Icon = icon,
                Title = L[title],
                Text = text,
                ActionText = actionText is null ? null : L[actionText].Value,
                ActionUrl = actionUrl
            });
    }
}
