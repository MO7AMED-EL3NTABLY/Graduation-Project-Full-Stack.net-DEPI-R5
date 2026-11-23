using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.DAL.Constants;

namespace MVC.PL.Areas.Admin.Controllers
{
    // الصفحة الرئيسية لـ Admin - placeholder لحد ما صاحب الـ module يكملها
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
