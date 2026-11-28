using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace MVC.PL.ViewModels.Accounts
{
    // Checkbox لازم يتعلّم عليه (زي الموافقة على الشروط)
    // [Range(typeof(bool), "true", "true")] بيفشل في الـ client validation على طول، عشان كده الـ attribute ده
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MustBeTrueAttribute : ValidationAttribute, IClientModelValidator
    {
        public override bool IsValid(object? value) => value is true;

        public void AddValidation(ClientModelValidationContext context)
        {
            var factory = context.ActionContext.HttpContext.RequestServices.GetService(typeof(IStringLocalizerFactory)) as IStringLocalizerFactory;
            var message = factory?.Create(typeof(SharedResource))[ErrorMessage ?? string.Empty].Value ?? ErrorMessage ?? string.Empty;

            context.Attributes["data-val"] = "true";
            context.Attributes["data-val-mustbetrue"] = message;
        }
    }
}
