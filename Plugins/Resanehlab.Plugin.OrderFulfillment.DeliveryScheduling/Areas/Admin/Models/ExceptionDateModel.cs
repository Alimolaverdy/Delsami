using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models
{
    public record ExceptionDateModel : BaseNopEntityModel, ILocalizedModel<ExceptionDateLocalizedModel>
    {
        public ExceptionDateModel()
        {
            Locales = new List<ExceptionDateLocalizedModel>();
            AvailableDeliveryTimeRanges = new List<AvailableDeliveryTimeRangeModel>();
            TimeRanges = new List<TimeRangeModel>();
            AvailableShippingMethods = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
            IsCustomTimeRange = false;
        }


        [NopResourceDisplayName("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Fields.Title")]
        public string Title { get; set; }

        [UIHint("Date")]
        [NopResourceDisplayName("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Fields.DateOnUtc")]
        public DateTime DateOnUtc { get; set; }

        public string DateOnUtcStr
        {
            get
            {
                return DateOnUtc.FormatDateTimeAsync().GetAwaiter().GetResult();
            }
        }

        public int? StoreId { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Fields.SelectedShippingMethod")]

        public int? SelectedShippingMethodId { get; set; }

        //public int SelectedTimeRangeId { get; set; }

        //public int Capacity { get; set; }

        public AvailableDeliveryTimeRangeModel ExceptionTimeRange { get; set; }
        public IList<AvailableDeliveryTimeRangeModel> ExceptionTimeRangeList { get; set; } = new List<AvailableDeliveryTimeRangeModel>();
        //public IList<int> ShippingMethodIds { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Fields.IsCustomTimeRange")]
        public bool IsCustomTimeRange { get; set; } 

        public IList<AvailableDeliveryTimeRangeModel> AvailableDeliveryTimeRanges { get; set; }

        public IList<TimeRangeModel> TimeRanges { get; set; }

        public IList<SelectListItem> AvailableShippingMethods { get; set; }

        public IList<ExceptionDateLocalizedModel> Locales { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }
    }

    public partial class ExceptionDateLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Fields.Title")]
        public string Title { get; set; }
    }
}
