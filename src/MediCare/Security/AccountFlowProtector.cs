using Microsoft.AspNetCore.DataProtection;

namespace MVC.PL.Security
{
    // Protects the userId during verification and password recovery steps without the user being signed in
    // - Verify cookie: the account that is still verifying email/phone (not signed in until it finishes)
    // - Reset ticket: the account resetting its password (hidden field in the form)
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
                return null; // Tampered with or expired
            }
        }
    }
}
