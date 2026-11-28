/*——- Omar Hussein ——-*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MVC.BLL.Interfaces;
using MVC.DAL.Constants;
using MVC.DAL.Entities.Accounts;
using MVC.PL.Security;
using MVC.PL.ViewModels.Accounts;

namespace MVC.PL.Areas.Doctor.Controllers
{
    // Professional profile: البيانات العادية بتتعدل علطول، والمحمية بطلب للأدمن، والمستندات
    [Area("Doctor")]
    [Authorize(Roles = AppRoles.Doctor)]
    [AutoValidateAntiforgeryToken]
    public class ProfileController : Controller
    {
        private readonly IDoctorProfileService _profiles;
        private readonly UserManager<ApplicationUser> _users;
        private readonly SignInManager<ApplicationUser> _signIn;
        private readonly IStringLocalizer<SharedResource> L;

        public ProfileController(
            IDoctorProfileService profiles,
            UserManager<ApplicationUser> users,
            SignInManager<ApplicationUser> signIn,
            IStringLocalizer<SharedResource> localizer)
        {
            _profiles = profiles;
            _users = users;
            _signIn = signIn;
            L = localizer;
        }

        private string DoctorId => _users.GetUserId(User)!;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var page = await BuildPageAsync();
            return page is null ? NotFound() : View(page);
        }

        [HttpPost]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public async Task<IActionResult> BasicInfo([Bind(Prefix = "BasicInfo")] BasicInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _profiles.UpdateBasicInfoAsync(DoctorId, model.FullName, model.PracticeStartYear!.Value, model.Bio, await model.Photo.ToFileUploadAsync());
                if (result.Succeeded)
                {
                    var user = await _users.FindByIdAsync(DoctorId);
                    if (user is not null)
                        await _signIn.RefreshSignInAsync(user); // الاسم في الـ Topbar
                    this.Flash(L["Your changes were saved."]);
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddErrors(result, L);
            }
            return await RedisplayAsync(null, p => p.BasicInfo = model);
        }

        [HttpPost]
        public async Task<IActionResult> RequestChange([Bind(Prefix = "RequestChange")] RequestChangeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _profiles.RequestChangeAsync(DoctorId, model.RequestType, model.RequestedValue);
                if (result.Succeeded)
                {
                    this.Flash(L["Your request was sent to the admin."]);
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddErrors(result, L);
            }
            return await RedisplayAsync("change-" + model.RequestType, p => p.RequestChange = model);
        }

        [HttpPost]
        [RequestSizeLimit(11 * 1024 * 1024)]
        public async Task<IActionResult> UploadDocument([Bind(Prefix = "Upload")] UploadDocumentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _profiles.UploadDocumentAsync(DoctorId, model.DocumentType, await model.File.ToFileUploadAsync());
                if (result.Succeeded)
                {
                    this.Flash(L["Your document was uploaded."]);
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddErrors(result, L);
            }
            return await RedisplayAsync("upload", p => p.Upload = new UploadDocumentViewModel { DocumentType = model.DocumentType });
        }

        [HttpPost]
        public async Task<IActionResult> AddSurgery([Bind(Prefix = "Surgery")] AddSurgeryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _profiles.AddSurgeryAsync(DoctorId, model.SurgeryName, model.Description);
                if (result.Succeeded)
                    return RedirectToAction(nameof(Index));
                ModelState.AddErrors(result, L);
            }
            return await RedisplayAsync("surgery", p => p.Surgery = model);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveSurgery(Guid id)
        {
            var result = await _profiles.RemoveSurgeryAsync(DoctorId, id);
            if (!result.Succeeded)
                this.FlashResult(result, string.Empty, L);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddExperience([Bind(Prefix = "Experience")] AddWorkExperienceViewModel model)
        {
            if (ModelState.IsValid)
            {
                var toYear = model.CurrentlyWorking ? null : model.ToYear;
                var result = await _profiles.AddWorkExperienceAsync(DoctorId, model.Organization, model.Position, model.FromYear!.Value, toYear);
                if (result.Succeeded)
                    return RedirectToAction(nameof(Index));
                ModelState.AddErrors(result, L);
            }
            return await RedisplayAsync("experience", p => p.Experience = model);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveExperience(Guid id)
        {
            var result = await _profiles.RemoveWorkExperienceAsync(DoctorId, id);
            if (!result.Succeeded)
                this.FlashResult(result, string.Empty, L);
            return RedirectToAction(nameof(Index));
        }

        private async Task<DoctorProfilePageViewModel?> BuildPageAsync()
        {
            var profile = await _profiles.GetAsync(DoctorId);
            if (profile is null)
                return null;

            return new DoctorProfilePageViewModel
            {
                Profile = profile,
                BasicInfo = new BasicInfoViewModel
                {
                    FullName = profile.FullName,
                    PracticeStartYear = profile.PracticeStartYear,
                    Bio = profile.Bio
                }
            };
        }

        private async Task<IActionResult> RedisplayAsync(string? modal, Action<DoctorProfilePageViewModel> apply)
        {
            var page = await BuildPageAsync();
            if (page is null)
                return NotFound();
            page.OpenModal = modal;
            apply(page);
            return View(nameof(Index), page);
        }
    }
}
/*————*/
