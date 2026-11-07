using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models;
using System.Collections.Generic;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Models
{
    public class AvailableDeliveryDateTimeTableModel
    {
        public List<AvailableDeliveryDateTimesTableModel> AvailableDeliveryDateTimesTable { get; set; }

        public bool IsOpc { get; set; }

        public bool IsDisabled { get; set; }

        public bool HideUnselectableDays { get; set; }
    }
}
