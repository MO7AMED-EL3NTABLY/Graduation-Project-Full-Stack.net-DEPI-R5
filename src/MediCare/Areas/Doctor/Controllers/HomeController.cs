using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.DAL.Constants;

namespace MVC.PL.Areas.Doctor.Controllers
{
    // Doctor home page - placeholder until the module owner completes it
    [Area("Doctor")]
    [Authorize(Roles = AppRoles.Doctor)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
