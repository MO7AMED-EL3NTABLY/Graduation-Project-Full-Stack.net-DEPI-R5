using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.DAL.Constants;

namespace MVC.PL.Areas.Admin.Controllers
{
    // Admin home page - placeholder until the module owner completes it
    [Area("Admin")]
    [Authorize(Roles = AppRoles.Admin)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
