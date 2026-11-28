using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MVC.BLL.Interfaces;
using MVC.DAL.Constants;
using MVC.DAL.Entities.Accounts;
using MVC.DAL.Enums;
using MVC.PL.Security;
using MVC.PL.ViewModels.Accounts;

namespace MVC.PL.Areas.Admin.Controllers
{
    // Requests to change protected data (specialty / license / university / graduation year)
    [Area("Admin")]
    [Authorize(Roles = AppRoles.Admin)]
    [AutoValidateAntiforgeryToken]
    public class ChangeRequestsController : Controller
    {
        private readonly IProfileChangeRequestService _requests;
        private readonly UserManager<ApplicationUser> _users;
        private readonly IStringLocalizer<SharedResource> L;

        public ChangeRequestsController(IProfileChangeRequestService requests, UserManager<ApplicationUser> users, IStringLocalizer<SharedResource> localizer)
        {
            _requests = requests;
            _users = users;
            L = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ApprovalStatus status = ApprovalStatus.Pending) =>
            View(new ChangeRequestsPageViewModel
            {
                Status = status,
                Counts = await _requests.GetCountsAsync(),
                Rows = await _requests.GetListAsync(status)
            });

        [HttpPost]
        public async Task<IActionResult> Approve(Guid id)
        {
            this.FlashResult(await _requests.ApproveAsync(_users.GetUserId(User)!, id), "The request was approved.", L);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Reject(Guid id, string? reason)
        {
            this.FlashResult(await _requests.RejectAsync(_users.GetUserId(User)!, id, reason), "The request was rejected.", L);
            return RedirectToAction(nameof(Index));
        }
    }
}
