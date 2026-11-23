using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace MVC.PL.Controllers
{
    // تبديل لغة الواجهة (عربي / إنجليزي) - بيحفظ الاختيار في cookie لمدة سنة
    public class CultureController : Controller
    {
        // ar-EG (مش ar بس) عشان التواريخ تبقى ميلادي مش هجري
        private static readonly string[] SupportedCultures = { "ar-EG", "en-US" };

        [HttpGet]
        public IActionResult Set(string culture, string? returnUrl)
        {
            if (!SupportedCultures.Contains(culture))
                culture = "en-US";

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), IsEssential = true, HttpOnly = true });

            // نرجع لنفس الصفحة بس لو الرابط جوه الموقع (حماية من open redirect)
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
