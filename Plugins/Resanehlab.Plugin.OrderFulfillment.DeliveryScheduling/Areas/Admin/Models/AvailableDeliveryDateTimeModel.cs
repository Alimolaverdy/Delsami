using Nop.Web.Framework.Models;
using System;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models
{
    public class AvailableDeliveryDateTimeModel
    {
        public int ShippingMethodCapacityId { get; set; }

        public int Capacity { get; set; }

        public int? DayOfWeek { get; set; }

        public DateTime StartDateOnUtc { get; set; }

        public DateTime? EndDateOnUtc { get; set; }

        public string Time { get; set; }

        public TimeSpan StartHour { get; set; }

        public TimeSpan EndHour { get; set; }

        public int DisplayOrder { get; set; }
        //public int ExceptionDateId { get; set; }



        public DateTime? ExceptionDate { get; set; }

        public int ShippingMethodId { get; set; }

        public int AvailableDeliveryTimeRangeId { get; set; }
    }
}
