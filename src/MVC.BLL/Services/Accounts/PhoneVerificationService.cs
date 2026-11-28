using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVC.BLL.Common;
using MVC.BLL.Interfaces;
using MVC.DAL.Dbcontexts;
using MVC.DAL.Entities.Accounts;

namespace MVC.BLL.Services.Accounts
{
    // Phone verification code - PhoneVerificationCodes table
    // The code itself is never stored (only its hash), and there is one active code per user
    public class PhoneVerificationService : IPhoneVerificationService
    {
        public const int CodeLifetimeMinutes = 10;
        public const int MaxFailedAttempts = 5;
        public const int ResendCooldownSeconds = 60;

        private readonly MediCareDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISmsSender _sms;

        public PhoneVerificationService(MediCareDbContext db, UserManager<ApplicationUser> userManager, ISmsSender sms)
        {
            _db = db;
            _userManager = userManager;
            _sms = sms;
        }

        public async Task<ServiceResult> SendCodeAsync(string userId, string targetPhoneNumber)
        {
            if (!ContactFormat.TryNormalizePhone(targetPhoneNumber, out var phone))
                return ServiceResult.Fail("Enter a valid Egyptian mobile number.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return ServiceResult.Fail("Account not found.");

            if (user.PhoneNumberConfirmed && user.PhoneNumber == phone)
                return ServiceResult.Fail("This is already your verified phone number.");

            if (await PhoneInUseAsync(phone, userId))
                return ServiceResult.Fail("This phone number is already used by another account.");

            var now = DateTimeOffset.UtcNow;
            var activeCodes = await ActiveCodes(userId).ToListAsync();
            var latest = activeCodes.OrderByDescending(c => c.CreatedAtUtc).FirstOrDefault();
            if (latest is not null && (now - latest.CreatedAtUtc).TotalSeconds < ResendCooldownSeconds)
                return ServiceResult.Fail("Please wait a minute before requesting a new code.");

            var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

            // The old code is invalidated and the new one created in the same transaction
            await using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                foreach (var old in activeCodes)
                    old.InvalidatedAtUtc = now;

                _db.PhoneVerificationCodes.Add(new PhoneVerificationCode
                {
                    UserId = userId,
                    CodeHash = Hash(userId, code),
                    TargetPhoneNumber = phone,
                    CreatedAtUtc = now,
                    ExpiresAtUtc = now.AddMinutes(CodeLifetimeMinutes),
                    FailedAttempts = 0
                });

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }

            // Send after the commit, not inside it
            await _sms.SendAsync(phone, $"MediCare: your verification code is {code}. It expires in {CodeLifetimeMinutes} minutes.");
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> ResendCodeAsync(string userId)
        {
            var latest = await ActiveCodes(userId).OrderByDescending(c => c.CreatedAtUtc).FirstOrDefaultAsync();
            if (latest is not null)
                return await SendCodeAsync(userId, latest.TargetPhoneNumber);

            // No active code: send to the registered number if it isn't verified yet
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null || string.IsNullOrEmpty(user.PhoneNumber) || user.PhoneNumberConfirmed)
                return ServiceResult.Fail("There is no phone number waiting for verification.");

            return await SendCodeAsync(userId, user.PhoneNumber);
        }

        public async Task<ServiceResult> VerifyCodeAsync(string userId, string code)
        {
            code = new string((code ?? string.Empty).Where(char.IsDigit).ToArray());
            if (code.Length != 6)
                return ServiceResult.Fail("Enter the 6-digit code.");

            var now = DateTimeOffset.UtcNow;
            var latest = await ActiveCodes(userId).OrderByDescending(c => c.CreatedAtUtc).FirstOrDefaultAsync();
            if (latest is null)
                return ServiceResult.Fail("No active code. Please request a new one.");
            if (latest.ExpiresAtUtc <= now)
                return ServiceResult.Fail("This code has expired. Please request a new one.");
            if (latest.FailedAttempts >= MaxFailedAttempts)
                return ServiceResult.Fail("Too many incorrect attempts. Please request a new code.");

            var expected = Convert.FromHexString(latest.CodeHash);
            var actual = Convert.FromHexString(Hash(userId, code));
            if (!CryptographicOperations.FixedTimeEquals(expected, actual))
            {
                latest.FailedAttempts++;
                await _db.SaveChangesAsync();
                return latest.FailedAttempts >= MaxFailedAttempts
                    ? ServiceResult.Fail("Too many incorrect attempts. Please request a new code.")
                    : ServiceResult.Fail("Incorrect code. Please try again.");
            }

            if (await PhoneInUseAsync(latest.TargetPhoneNumber, userId))
                return ServiceResult.Fail("This phone number is already used by another account.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return ServiceResult.Fail("Account not found.");

            // The number changes only after the code is verified - in the same transaction as using the code
            await using var transaction = await _db.Database.BeginTransactionAsync();
            latest.UsedAtUtc = now;
            user.PhoneNumber = latest.TargetPhoneNumber;
            user.PhoneNumberConfirmed = true;
            var update = await _userManager.UpdateAsync(user);
            if (!update.Succeeded)
                return ServiceResult.Fail(update.Errors.Select(e => e.Description));
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return ServiceResult.Success();
        }

        public async Task<(string? TargetPhoneNumber, int ResendInSeconds)> GetActiveCodeInfoAsync(string userId)
        {
            var latest = await ActiveCodes(userId).OrderByDescending(c => c.CreatedAtUtc).FirstOrDefaultAsync();
            if (latest is null)
                return (null, 0);

            var elapsed = (DateTimeOffset.UtcNow - latest.CreatedAtUtc).TotalSeconds;
            var wait = (int)Math.Ceiling(Math.Max(0, ResendCooldownSeconds - elapsed));
            return (latest.TargetPhoneNumber, wait);
        }

        private IQueryable<PhoneVerificationCode> ActiveCodes(string userId) =>
            _db.PhoneVerificationCodes.Where(c => c.UserId == userId && c.UsedAtUtc == null && c.InvalidatedAtUtc == null);

        private Task<bool> PhoneInUseAsync(string phone, string userId) =>
            _db.Users.AnyAsync(u => u.Id != userId && u.PhoneNumber == phone && u.PhoneNumberConfirmed);

        private static string Hash(string userId, string code) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{userId}:{code}")));
    }
}
