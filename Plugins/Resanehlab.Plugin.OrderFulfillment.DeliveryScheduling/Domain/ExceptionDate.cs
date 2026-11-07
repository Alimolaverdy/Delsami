using Nop.Core;
using Nop.Core.Domain.Localization;
using System;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain
{
    public class ExceptionDate : BaseEntity, ILocalizedEntity
    {
        public string Title { get; set; }

        public DateTime DateOnUtc { get; set; }

        public int StoreId { get; set; }

        public bool Deleted { get; set; }

        public bool IsCustomTimeRange { get; set; }

       // public string ShippingMethodIds { get; set; }
    }
}
