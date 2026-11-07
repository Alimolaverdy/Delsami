using System;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models
{
    public class GetPlacedOrdersCountModel
    {
        public int ShippingMethodId { get; set; }

        public DateTime DeliveryDateOnUtc { get; set; }

        public int AvailableDeliveryTimeRangeId { get; set; }

        public int OrdersCount { get; set; }
    }
}
