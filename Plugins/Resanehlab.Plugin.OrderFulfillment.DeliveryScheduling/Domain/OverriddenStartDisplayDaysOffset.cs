using Nop.Core;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain
{
    public class OverriddenStartDisplayDaysOffset : BaseEntity
    {
        public int CategoryId { get; set; }

        public int StartDisplayDaysOffset { get; set; }
    }
}
