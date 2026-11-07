using Nop.Core;
using Nop.Core.Domain.Localization;
using System;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain
{
    public class AvailableDeliveryTimeRange : BaseEntity, ILocalizedEntity
    {
        public string Time { get; set; }

        public TimeSpan StartHour { get; set; }

        public TimeSpan EndHour { get; set; }

        public int StoreId { get; set; }

        public int DisplayOrder { get; set; }

        public bool Deleted { get; set; }

        public string DaysOfWeek { get; set; }

        public string ShippingMethodIds { get; set; }

        public int? ExceptionDateId { get; set; }

    }
}
