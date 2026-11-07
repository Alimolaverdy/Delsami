using Nop.Core.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Settings
{
    public class DeliverySchedulingSetting :ISettings
    {
        public bool EnablePlugin { get; set; }

        public int NumberOfDisplayDays { get; set; }

        public int StartDisplayDaysOffset { get; set; } 

        public int StartHourOffsetIfDaysOffsetIsZero { get; set; }

        public string DisabledShippingMethodIds { get; set; }

        public IList<int> DisabledShippingMethodIdsAsInt
        {
            get
            {
                return string.IsNullOrWhiteSpace(DisabledShippingMethodIds) ? new List<int>() : DisabledShippingMethodIds.Split(',').Select(s => int.Parse(s)).ToList();
            }
        }

        public bool HideUnselectableDays { get; set; }

        public string DeliveryDateTimeFormatString { get; set; }

        public bool FormatDeliveryDateTimesBasedOnCustomerLanguage { get; set; }
    }
}

