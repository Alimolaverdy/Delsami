using Nop.Web.Framework.Models;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models
{
    public record OverriddenStartDisplayDaysOffsetModel : BaseNopEntityModel
    {
        public int CategoryId { get; set; }

        public int StartDisplayDaysOffset { get; set; }

        public string CategoryName { get; set; }
    }
}
