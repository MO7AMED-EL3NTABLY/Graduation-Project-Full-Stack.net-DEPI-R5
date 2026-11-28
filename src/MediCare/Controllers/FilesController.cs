/*——- Omar Hussein ——-*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC.BLL.Interfaces;
using MVC.DAL.Constants;
using MVC.DAL.Entities.Accounts;

namespace MVC.PL.Controllers
{
    // الملفات المحمية (صور البروفايل ومستندات الدكاترة) - مفيش رابط مباشر، كل طلب بيعدي على الصلاحيات
    public class FilesController : Controller
    {
        private readonly IFileAccessService _files;
        private readonly UserManager<ApplicationUser> _users;

        public FilesController(IFileAccessService files, UserManager<ApplicationUser> users)
        {
            _files = files;
            _users = users;
        }

        // صورة الدكتور المعتمد متاحة للكل، وغير كده لصاحبها والأدمن
        [HttpGet("/files/photo/{userId}")]
        public async Task<IActionResult> Photo(string userId)
        {
            var file = await _files.GetUserPhotoAsync(_users.GetUserId(User), User.IsInRole(AppRoles.Admin), userId);
            return file is null ? NotFound() : File(file.Content, file.ContentType);
        }

        // مستندات الدكتور: الدكتور صاحبها والأدمن بس
        [Authorize]
        [HttpGet("/files/document/{id:guid}")]
        public async Task<IActionResult> Document(Guid id, bool download = false)
        {
            var file = await _files.GetDoctorDocumentAsync(_users.GetUserId(User), User.IsInRole(AppRoles.Admin), id);
            if (file is null)
                return NotFound();

            Response.Headers["X-Content-Type-Options"] = "nosniff";
            Response.Headers["Cache-Control"] = "private, no-store";
            return download
                ? File(file.Content, file.ContentType, file.FileName)
                : File(file.Content, file.ContentType);
        }
    }
}
/*————*/
