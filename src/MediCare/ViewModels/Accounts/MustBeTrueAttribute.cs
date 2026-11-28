using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace MVC.PL.ViewModels.Accounts
{
    // A checkbox that must be ticked (like accepting the terms)
    // [Range(typeof(bool), "true", "true")] always fails client validation, hence this attribute
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
