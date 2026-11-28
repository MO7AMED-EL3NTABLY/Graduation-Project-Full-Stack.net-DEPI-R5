/*——- Omar Hussein ——-*/
using Microsoft.AspNetCore.Identity;
using MVC.BLL.Common;
using MVC.BLL.DTOs.Accounts;
using MVC.BLL.Interfaces;
using MVC.DAL.Dbcontexts;
using MVC.DAL.Entities.Accounts;

namespace MVC.BLL.Services.Accounts
{
    // صفحة Settings للمريض والدكتور: الاسم والصورة والإيميل والموبايل والباسورد
    public class AccountSettingsService : IAccountSettingsService
    {
        private readonly MediCareDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileStorage _files;
        private readonly IEmailSender _email;
        private readonly IPhoneVerificationService _phoneVerification;

        public AccountSettingsService(
            MediCareDbContext db,
            UserManager<ApplicationUser> userManager,
            IFileStorage files,
            IEmailSender email,
            IPhoneVerificationService phoneVerification)
        {
            _db = db;
            _userManager = userManager;
            _files = files;
            _email = email;
            _phoneVerification = phoneVerification;
        }

        public async Task<AccountSettingsDto?> GetAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return null;

            return new AccountSettingsDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = ContactFormat.FormatPhone(user.PhoneNumber),
                PhoneConfirmed = user.PhoneNumberConfirmed,
                HasPhoto = !string.IsNullOrEmpty(user.ProfilePhotoStorageKey)
            };
        }

        public async Task<ServiceResult> UpdatePatientProfileAsync(string userId, string fullName, FileUpload? newPhoto)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return ServiceResult.Fail("Full name is required.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return ServiceResult.Fail("Account not found.");

            return await UpdateNameAndPhotoAsync(_userManager, _files, user, fullName, newPhoto);
        }

        // مشتركة مع Professional profile بتاع الدكتور
        internal static async Task<ServiceResult> UpdateNameAndPhotoAsync(
            UserManager<ApplicationUser> userManager, IFileStorage files, ApplicationUser user, string fullName, FileUpload? newPhoto)
        {
            string? newKey = null;
            if (newPhoto is not null && newPhoto.Length > 0)
            {
                var photoError = FileRules.ValidatePhoto(newPhoto);
                if (photoError is not null)
                    return ServiceResult.Fail(photoError);
                newKey = await files.SaveAsync(newPhoto, AccountService.PhotosFolder);
            }

            var oldKey = user.ProfilePhotoStorageKey;
            user.FullName = fullName.Trim();
            if (newKey is not null)
                user.ProfilePhotoStorageKey = newKey;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                if (newKey is not null)
                    files.Delete(newKey);
                return ServiceResult.Fail(result.Errors.Select(e => e.Description));
            }

            // الصورة القديمة بتتمسح بعد ما الجديدة تتحفظ
            if (newKey is not null && !string.IsNullOrEmpty(oldKey))
                files.Delete(oldKey);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> RequestEmailChangeAsync(string userId, string newEmail, EmailChangeLinkBuilder link)
        {
            newEmail = (newEmail ?? string.Empty).Trim();
            if (!ContactFormat.LooksLikeEmail(newEmail))
                return ServiceResult.Fail("Enter a valid email address.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return ServiceResult.Fail("Account not found.");
            if (string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase))
                return ServiceResult.Fail("This is already your email.");
            if (await _userManager.FindByEmailAsync(newEmail) is not null)
                return ServiceResult.Fail("An account with this email already exists.");

            // الإيميل الحالي بيفضل شغال، واللينك بيروح للإيميل الجديد (صالح 24 ساعة)
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
            await _email.SendAsync(newEmail, "Confirm your new MediCare email",
                $"Hi {user.FullName},\n\nConfirm your new email by opening this link (valid for 24 hours):\n{link(user.Id, newEmail, token)}\n\nYour current email stays active until you confirm.");
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> ConfirmEmailChangeAsync(string userId, string newEmail, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null || string.IsNullOrEmpty(newEmail) || string.IsNullOrEmpty(token))
                return ServiceResult.Fail("This confirmation link is invalid or has expired.");

            var owner = await _userManager.FindByEmailAsync(newEmail);
            if (owner is not null && owner.Id != user.Id)
                return ServiceResult.Fail("An account with this email already exists.");

            // الإيميل واسم المستخدم (UserName = Email) بيتغيروا مع بعض
            await using var transaction = await _db.Database.BeginTransactionAsync();
            var change = await _userManager.ChangeEmailAsync(user, newEmail, token);
            if (!change.Succeeded)
                return ServiceResult.Fail("This confirmation link is invalid or has expired.");

            var rename = await _userManager.SetUserNameAsync(user, newEmail);
            if (!rename.Succeeded)
                return ServiceResult.Fail(rename.Errors.Select(e => e.Description));

            await transaction.CommitAsync();
            return ServiceResult.Success();
        }

        public Task<ServiceResult> RequestPhoneChangeAsync(string userId, string newPhoneNumber) =>
            _phoneVerification.SendCodeAsync(userId, newPhoneNumber);

        public async Task<ServiceResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return ServiceResult.Fail("Account not found.");
            if (currentPassword == newPassword)
                return ServiceResult.Fail("Your new password must be different from the old one.");

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
                return ServiceResult.Success();

            if (result.Errors.Any(e => e.Code == "PasswordMismatch"))
                return ServiceResult.Fail("Current password is incorrect.");
            return ServiceResult.Fail(result.Errors.Select(e => e.Description));
        }
    }
}
/*————*/
