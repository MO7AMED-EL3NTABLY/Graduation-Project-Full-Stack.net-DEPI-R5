/*——- Omar Hussein ——-*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using MVC.BLL.Common;

namespace MVC.PL.Security
{
    public static class ControllerExtensions
    {
        public const string SuccessKey = "mc-success";
        public const string ErrorKey = "mc-error";

        // أخطاء الـ BLL مفاتيح إنجليزي، بنترجمها هنا
        public static void AddErrors(this ModelStateDictionary modelState, ServiceResult result, IStringLocalizer localizer)
        {
            foreach (var error in result.Errors)
                modelState.AddModelError(string.Empty, localizer[error]);
        }

        // رسالة تظهر بعد الـ Redirect (Post-Redirect-Get)
        public static void Flash(this Controller controller, string message, bool success = true) =>
            controller.TempData[success ? SuccessKey : ErrorKey] = message;

        public static void FlashResult(this Controller controller, ServiceResult result, string successMessage, IStringLocalizer localizer)
        {
            if (result.Succeeded)
                controller.Flash(localizer[successMessage]);
            else
                controller.Flash(string.Join(" ", result.Errors.Select(e => localizer[e].Value)), success: false);
        }
    }
}
/*————*/
