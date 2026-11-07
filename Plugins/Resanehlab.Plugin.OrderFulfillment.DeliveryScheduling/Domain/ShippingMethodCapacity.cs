using Nop.Core;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain
{
    public class ShippingMethodCapacity : BaseEntity
    {
        public int ShippingMethodId { get; set; }

        public int AvailableDeliveryDateTimeRangeId { get; set; }

        public int Capacity { get; set; }

        public bool Deleted { get; set; }
    }
}
