using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models
{
    public class TimeTableModel
    {
        public TimeTableModel()
        {
            AvailableStores = new List<SelectListItem>();
            TimeRangeLists = new List<List<TimeRangeModel>>();
            AvailableDeliveryTimeRanges = new List<AvailableDeliveryTimeRangeModel>();
            AvailableShippingMethods = new List<SelectListItem>();
            TimeRanges = new List<TimeRangeModel>();
        }

        public bool HasConfirmedTimeRange { get; set; }

        public List<AvailableDeliveryTimeRangeModel> AvailableDeliveryTimeRanges { get; set; }

        public IList<List<TimeRangeModel>> TimeRangeLists { get; set; }

        public IList<TimeRangeModel> TimeRanges { get; set; }

        public int? StoreId { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.TimeTable.Fields.ShippingMethod")]
        public int ShippingMethodId { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        public List<SelectListItem> AvailableShippingMethods { get; set; }
    }
}
