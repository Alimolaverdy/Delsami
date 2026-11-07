using Nop.Web.Framework.Models;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models
{
    public record OrderDeliveryModel : BaseNopEntityModel
    {
        public string DeliveryDate { get; set; }

        public string DeliveryTime { get; set; }

        public int OrderId { get; set; }

        public string ShippingMethodName { get; set; }

        public string StoreName { get; set; }

        public string OrderName { get; set; }
    }
}
