/*——- Omar Hussein ——-*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MVC.BLL.DTOs.Accounts;
using MVC.BLL.Interfaces;
using MVC.DAL.Constants;
using MVC.DAL.Entities.Accounts;
using MVC.PL.Security;
using MVC.PL.ViewModels.Accounts;

namespace MVC.PL.Areas.Admin.Controllers
{
    // اعتماد الأطباء: المراجعة الأولى (الرخصة + الشهادة مع بعض) والمستندات البديلة
    [Area("Admin")]
    [Authorize(Roles = AppRoles.Admin)]
    [AutoValidateAntiforgeryToken]
    public class DoctorApprovalsController : Controller
    {
        private readonly IDoctorApprovalService _approvals;
        private readonly UserManager<ApplicationUser> _users;
        private readonly IStringLocalizer<SharedResource> L;

        public DoctorApprovalsController(IDoctorApprovalService approvals, UserManager<ApplicationUser> users, IStringLocalizer<SharedResource> localizer)
        {
            _approvals = approvals;
            _users = users;
            L = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ApprovalTab tab = ApprovalTab.Initial, string? doctorId = null, Guid? documentId = null)
        {
            var page = await BuildPageAsync(tab, doctorId, documentId);
            return View(page);
        }

        [HttpPost]
        public async Task<IActionResult> Decide(ApprovalDecisionViewModel model)
        {
            if (string.IsNullOrEmpty(model.DoctorId))
                return RedirectToAction(nameof(Index));

            if (ModelState.IsValid)
            {
                var result = await _approvals.DecideInitialAsync(_users.GetUserId(User)!, model.DoctorId, model.Approve, model.RejectionReason);
                if (result.Succeeded)
                {
                    this.Flash(L[model.Approve ? "The doctor was approved." : "The application was rejected."]);
                    return RedirectToAction(nameof(Index), new { tab = ApprovalTab.Initial });
                }
                ModelState.AddErrors(result, L);
            }

            var page = await BuildPageAsync(ApprovalTab.Initial, model.DoctorId, null);
            ViewData["RejectionReason"] = model.RejectionReason;
            return View(nameof(Index), page);
        }

        [HttpPost]
        public async Task<IActionResult> DecideReplacement(ApprovalDecisionViewModel model)
        {
            if (model.DocumentId is null)
                return RedirectToAction(nameof(Index), new { tab = ApprovalTab.Replacements });

            if (ModelState.IsValid)
            {
                var result = await _approvals.DecideReplacementAsync(_users.GetUserId(User)!, model.DocumentId.Value, model.Approve, model.RejectionReason);
                if (result.Succeeded)
                {
                    this.Flash(L[model.Approve ? "The replacement was approved." : "The replacement was rejected."]);
                    return RedirectToAction(nameof(Index), new { tab = ApprovalTab.Replacements });
                }
                ModelState.AddErrors(result, L);
            }

            var page = await BuildPageAsync(ApprovalTab.Replacements, null, model.DocumentId);
            ViewData["RejectionReason"] = model.RejectionReason;
            return View(nameof(Index), page);
        }

        private async Task<DoctorApprovalsPageViewModel> BuildPageAsync(ApprovalTab tab, string? doctorId, Guid? documentId)
        {
            var page = new DoctorApprovalsPageViewModel
            {
                Tab = tab,
                Counts = await _approvals.GetCountsAsync(),
                Items = await _approvals.GetListAsync(tab)
            };

            // لو مفيش اختيار بنفتح أول عنصر في القائمة
            if (tab == ApprovalTab.Replacements)
            {
                page.SelectedDocumentId = documentId ?? page.Items.FirstOrDefault()?.DocumentId;
                if (page.SelectedDocumentId is not null)
                    page.Replacement = await _approvals.GetReplacementReviewAsync(page.SelectedDocumentId.Value);
            }
            else
            {
                page.SelectedDoctorId = doctorId ?? page.Items.FirstOrDefault()?.DoctorId;
                if (page.SelectedDoctorId is not null)
                    page.Doctor = await _approvals.GetDoctorReviewAsync(page.SelectedDoctorId);
            }

            return page;
        }
    }
}
/*————*/
