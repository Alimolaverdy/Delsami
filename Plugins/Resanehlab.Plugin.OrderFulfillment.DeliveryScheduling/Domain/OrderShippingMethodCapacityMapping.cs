using Nop.Core;
using System;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain
{
    public class OrderShippingMethodCapacityMapping : BaseEntity
    {
        public int OrderId { get; set; }

        public int ShippingMethodCapacityId { get; set; }

        public DateTime DeliveryDateOnUtc { get; set; }

        public string Time { get; set; }

        public TimeSpan StartHour { get; set; }

        public TimeSpan EndHour { get; set; }

        public int  ShippingMethodId { get; set; }
    }
}
