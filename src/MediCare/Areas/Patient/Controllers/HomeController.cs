using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.DAL.Constants;

namespace MVC.PL.Areas.Patient.Controllers
{
    // الصفحة الرئيسية لـ Patient - placeholder لحد ما صاحب الـ module يكملها
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
