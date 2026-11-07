using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Validators
{
    public class TimeRangeValidator : BaseNopValidator<TimeRangeModel>
    {
        public TimeRangeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Capacity).NotNull().WithMessageAwait(localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.TimeRange.Fields.Capacity.Required"));
        }
    }
}
