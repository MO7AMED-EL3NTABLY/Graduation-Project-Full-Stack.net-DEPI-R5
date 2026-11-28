using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.DAL.Constants;

namespace MVC.PL.Areas.Patient.Controllers
{
    // Patient home page - placeholder until the module owner completes it
    [Area("Patient")]
    [Authorize(Roles = AppRoles.Patient)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
