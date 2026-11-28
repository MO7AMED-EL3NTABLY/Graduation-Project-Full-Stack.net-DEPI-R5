using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace MVC.PL.Controllers
{
    // Switches the UI language (Arabic / English) - saves the choice in a cookie for one year
    public class CultureController : Controller
    {
        // ar-EG (not just ar) so dates are Gregorian, not Hijri
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

            // Go back to the same page only if the URL is local (open redirect protection)
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
