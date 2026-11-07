namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models
{
    public class TimeRangeModel
    {
        public int Capacity { get; set; }

        public int DayOfWeek { get; set; }

        public int ShippingMethodCapacityId { get; set; }

        public int AvailableDeliveryTimeRangeId { get; set; }
    }
}
