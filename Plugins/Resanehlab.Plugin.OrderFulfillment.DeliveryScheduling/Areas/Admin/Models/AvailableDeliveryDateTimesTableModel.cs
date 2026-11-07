using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models
{
    public class AvailableDeliveryDateTimesTableModel
    {
        public AvailableDeliveryDateTimesTableModel()
        {
            Times = new List<AvailableDeliveryTimesTableModel>();
        }

        public DateTime DeliveryDateOnUtc { get; set; }

        public string DeliveryDateString { get; set; }

        [NotMapped]
        public IList<AvailableDeliveryTimesTableModel> Times { get; set; }
    }

    public class AvailableDeliveryTimesTableModel
    {
        public string Time { get; set; }

        public int ShippingMethodCapacityId { get; set; }

        public int AvailableCapacities { get; set; }

        public int AvailableDeliveryTimeRangeId { get; set; }
    }
}
