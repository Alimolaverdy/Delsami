using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public ConfigurationModel()
        {
            SelectedDisabledShippingMethodIds = new List<int>();
            AvailableShippingMethods = new List<SelectListItem>();
        }

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.EnablePlugin")]
        public bool EnablePlugin { get; set; }
        public bool EnablePlugin_OverrideForStore { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.NumberOfDisplayDays")]
        public int NumberOfDisplayDays { get; set; }
        public bool NumberOfDisplayDays_OverrideForStore { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.StartDisplayDaysOffset")]
        public int StartDisplayDaysOffset { get; set; }
        public bool StartDisplayDaysOffset_OverrideForStore { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.StartHourOffsetIfDaysOffsetIsZero")]
        public int StartHourOffsetIfDaysOffsetIsZero { get; set; }
        public bool StartHourOffsetIfDaysOffsetIsZero_OverrideForStore { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.DisabledShippingMethodIds")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedDisabledShippingMethodIds { get; set; }

        public IList<SelectListItem> AvailableShippingMethods { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.HideUnselectableDays")]
        public bool HideUnselectableDays { get; set; }
        public bool HideUnselectableDays_OverrideForStore { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.DeliveryDateTimeFormatString")]
        public string DeliveryDateTimeFormatString { get; set; }
        public bool DeliveryDateTimeFormatString_OverrideForStore { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.FormatDeliveryDateTimesBasedOnCustomerLanguage")]
        public bool FormatDeliveryDateTimesBasedOnCustomerLanguage { get; set; }
        public bool FormatDeliveryDateTimesBasedOnCustomerLanguage_OverrideForStore { get; set; }
    }
}