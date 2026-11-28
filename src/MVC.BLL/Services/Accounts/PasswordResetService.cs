using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using MVC.BLL.Common;
using MVC.BLL.DTOs.Accounts;
using MVC.BLL.Interfaces;
using MVC.DAL.Constants;
using MVC.DAL.Entities.Accounts;

namespace MVC.BLL.Services.Accounts
{
    // Forgot password: email link (Identity token - 1 hour) or SMS code (Identity phone token)
    // No reset table: codes come from Identity token providers, and the attempt count is in the memory cache
    public class PasswordResetService : IPasswordResetService
    {
        private const string SmsPurpose = "MediCare.ResetPassword";
        private const int SmsCodeLifetimeMinutes = 10;
        private const int MaxFailedAttempts = 5;
        private const int ResendCooldownSeconds = 60;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountService _accounts;
        private readonly IEmailSender _email;
        private readonly ISmsSender _sms;
        private readonly IMemoryCache _cache;

        public PasswordResetService(
            UserManager<ApplicationUser> userManager,
            IAccountService accounts,
            IEmailSender email,
            ISmsSender sms,
            IMemoryCache cache)
        {
            _userManager = userManager;
            _accounts = accounts;
            _email = email;
            _sms = sms;
            _cache = cache;
        }

        public async Task<ResetAccountDto?> FindAccountAsync(string email)
        {
            var userId = await _accounts.FindUserIdByEmailAsync(email);
            if (userId is null)
                return null;

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null || !user.IsActive)
                return null;

            return new ResetAccountDto
            {
                UserId = user.Id,
                MaskedEmail = ContactFormat.MaskEmail(user.Email),
                MaskedPhone = ContactFormat.MaskPhone(user.PhoneNumber),
                CanUseSms = await CanUseSmsAsync(user)
            };
        }

        public async Task<ServiceResult> SendEmailLinkAsync(string userId, AccountLinkBuilder resetLink)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null || !user.IsActive || string.IsNullOrEmpty(user.Email))
                return ServiceResult.Fail("Account not found.");

            // This token comes from the reset provider (valid for 1 hour - configured in Program.cs)
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var sent = await _email.SendAsync(user.Email, "Reset your MediCare password",
                EmailTemplates.ActionEmail(user.FullName,
                    "Reset your password",
                    "We received a request to reset the password of your MediCare account.",
                    "Reset password", resetLink(user.Id, token),
                    "This link is valid for 1 hour. If you didn't ask for this, you can ignore this email."));
            if (!sent)
                return ServiceResult.Fail("We couldn't send the email. Please try again later.");
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> SendSmsCodeAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null || !user.IsActive || !await CanUseSmsAsync(user))
                return ServiceResult.Fail("SMS reset isn't available for this account.");

            var now = DateTimeOffset.UtcNow;
            if (_cache.TryGetValue(CacheKey(userId), out SmsResetState? existing) && existing is not null
                && (now - existing.IssuedAtUtc).TotalSeconds < ResendCooldownSeconds)
                return ServiceResult.Fail("Please wait a minute before requesting a new code.");

            var code = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, SmsPurpose);
            _cache.Set(CacheKey(userId), new SmsResetState { IssuedAtUtc = now }, TimeSpan.FromMinutes(SmsCodeLifetimeMinutes));

            await _sms.SendAsync(user.PhoneNumber!, $"MediCare: your password reset code is {code}. It expires in {SmsCodeLifetimeMinutes} minutes.");
            return ServiceResult.Success();
        }

        public async Task<ServiceResult<string>> VerifySmsCodeAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null || !user.IsActive)
                return ServiceResult<string>.Fail("Account not found.");

            if (!_cache.TryGetValue(CacheKey(userId), out SmsResetState? state) || state is null)
                return ServiceResult<string>.Fail("This code has expired. Please request a new one.");
            if (state.FailedAttempts >= MaxFailedAttempts)
                return ServiceResult<string>.Fail("Too many incorrect attempts. Please request a new code.");

            code = new string((code ?? string.Empty).Where(char.IsDigit).ToArray());
            var valid = code.Length == 6
                && await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, SmsPurpose, code);
            if (!valid)
            {
                state.FailedAttempts++;
                return state.FailedAttempts >= MaxFailedAttempts
                    ? ServiceResult<string>.Fail("Too many incorrect attempts. Please request a new code.")
                    : ServiceResult<string>.Fail("Incorrect code. Please try again.");
            }

            _cache.Remove(CacheKey(userId));
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            return ServiceResult<string>.Success(resetToken);
        }

        public async Task<ServiceResult> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null || string.IsNullOrEmpty(token))
                return ServiceResult.Fail("This reset link is invalid or has expired.");

            if (await _userManager.CheckPasswordAsync(user, newPassword))
                return ServiceResult.Fail("Your new password must be different from the old one.");

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded)
                return ServiceResult.Success();

            // InvalidToken = the link expired or was already used
            if (result.Errors.Any(e => e.Code == "InvalidToken"))
                return ServiceResult.Fail("This reset link is invalid or has expired.");
            return ServiceResult.Fail(result.Errors.Select(e => e.Description));
        }

        private async Task<bool> CanUseSmsAsync(ApplicationUser user) =>
            user.PhoneNumberConfirmed
            && !string.IsNullOrEmpty(user.PhoneNumber)
            && !await _userManager.IsInRoleAsync(user, AppRoles.Admin);

        private static string CacheKey(string userId) => $"pwd-reset-sms:{userId}";

        private sealed class SmsResetState
        {
            public DateTimeOffset IssuedAtUtc { get; init; }
            public int FailedAttempts { get; set; }
        }
    }
}
