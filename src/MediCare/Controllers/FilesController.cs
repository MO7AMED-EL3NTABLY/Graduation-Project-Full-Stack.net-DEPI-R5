using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC.BLL.Interfaces;
using MVC.DAL.Constants;
using MVC.DAL.Entities.Accounts;

namespace MVC.PL.Controllers
{
    // Protected files (profile photos and doctor documents) - no direct link, every request goes through authorization
    public class FilesController : Controller
    {
        private readonly IFileAccessService _files;
        private readonly UserManager<ApplicationUser> _users;

        public FilesController(IFileAccessService files, UserManager<ApplicationUser> users)
        {
            _files = files;
            _users = users;
        }

        // An approved doctor's photo is public; otherwise only its owner and the admin
        [HttpGet("/files/photo/{userId}")]
        public async Task<IActionResult> Photo(string userId)
        {
            var file = await _files.GetUserPhotoAsync(_users.GetUserId(User), User.IsInRole(AppRoles.Admin), userId);
            return file is null ? NotFound() : File(file.Content, file.ContentType);
        }

        // Doctor documents: only the owning doctor and the admin
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
