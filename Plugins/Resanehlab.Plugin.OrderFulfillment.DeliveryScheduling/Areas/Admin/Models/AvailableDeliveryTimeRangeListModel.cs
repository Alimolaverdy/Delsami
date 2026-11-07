using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using System.Collections.Generic;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models
{
    public record AvailableDeliveryTimeRangeListModel : BaseNopEntityModel
    {
        public AvailableDeliveryTimeRangeListModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        public bool IsConfirmed { get; set; }

        public int ItemsCount { get; set; }

        public int? StoreId { get; set; }

        public List<SelectListItem> AvailableStores { get; set; }
    }
}
