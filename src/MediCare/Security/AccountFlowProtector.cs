/*——- Omar Hussein ——-*/
using Microsoft.AspNetCore.DataProtection;

namespace MVC.PL.Security
{
    // بيحمي الـ userId في خطوات التأكيد واسترجاع الباسورد من غير ما المستخدم يكون عامل Login
    // - Verify cookie: الحساب اللي لسه بيأكد الإيميل/الموبايل (مش Signed in لحد ما يخلص)
    // - Reset ticket: الحساب اللي بيعمل Reset للباسورد (Hidden field في الفورم)
    public class AccountFlowProtector
    {
        public const string VerifyCookieName = "MediCare.Verify";
        private static readonly TimeSpan VerifyLifetime = TimeSpan.FromHours(24);
        private static readonly TimeSpan ResetLifetime = TimeSpan.FromMinutes(30);

        private readonly ITimeLimitedDataProtector _verify;
        private readonly ITimeLimitedDataProtector _reset;

        public AccountFlowProtector(IDataProtectionProvider provider)
        {
            _verify = provider.CreateProtector("MediCare.Accounts.Verify").ToTimeLimitedDataProtector();
            _reset = provider.CreateProtector("MediCare.Accounts.Reset").ToTimeLimitedDataProtector();
        }

        public void SetVerifyCookie(HttpResponse response, string userId) =>
            response.Cookies.Append(VerifyCookieName, _verify.Protect(userId, VerifyLifetime), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.Add(VerifyLifetime)
            });

        public string? GetVerifyUserId(HttpRequest request) =>
            request.Cookies.TryGetValue(VerifyCookieName, out var value) ? Unprotect(_verify, value) : null;

        public void ClearVerifyCookie(HttpResponse response) => response.Cookies.Delete(VerifyCookieName);

        public string CreateResetTicket(string userId) => _reset.Protect(userId, ResetLifetime);

        public string? ReadResetTicket(string? ticket) => string.IsNullOrEmpty(ticket) ? null : Unprotect(_reset, ticket);

        private static string? Unprotect(ITimeLimitedDataProtector protector, string value)
        {
            try
            {
                return protector.Unprotect(value);
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                return null; // متلعب فيه أو انتهى
            }
        }
    }
}
/*————*/
