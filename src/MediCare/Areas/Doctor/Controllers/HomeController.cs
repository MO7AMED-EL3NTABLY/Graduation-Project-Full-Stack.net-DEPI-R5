using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.DAL.Constants;

namespace MVC.PL.Areas.Doctor.Controllers
{
    // الصفحة الرئيسية لـ Doctor - placeholder لحد ما صاحب الـ module يكملها
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
