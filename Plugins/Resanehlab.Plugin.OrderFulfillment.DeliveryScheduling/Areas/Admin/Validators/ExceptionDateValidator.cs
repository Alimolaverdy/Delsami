using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Validators
{
    public class ExceptionDateValidator : BaseNopValidator<ExceptionDateModel>
    {
        public ExceptionDateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.DateOnUtc).NotNull().WithMessageAwait(localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Fields.DateOnUtc.Required"));
        }
    }
}
