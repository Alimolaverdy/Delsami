using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models
{
    public record OrderDeliverySearchModel : BaseSearchModel
    {
        public OrderDeliverySearchModel()
        {
            AvailableShippingMethods = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
        }

        public List<SelectListItem> AvailableShippingMethods { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.Fields.FromDate")]
        [UIHint("DateNullable")]
        public DateTime? FromDate { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.Fields.ToDate")]
        [UIHint("DateNullable")]
        public DateTime? ToDate { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.Fields.SelectedShippingMethod")]
        public int? SelectedShippingMethodId { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.Fields.Store")]
        public int StoreId { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }
    }
}
