using Nop.Core;
using System;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain
{
    public class AvailableDeliveryDateTimeRange : BaseEntity
    {
        public int? DayOfWeek { get; set; }

        public int? ExceptionDateId { get; set; }

        public int AvailableDeliveryTimeRangeId { get; set; }

        public DateTime StartDateOnUtc { get; set; }

        public DateTime? EndDateOnUtc { get; set; }
    }
}
