using Nop.Web.Framework.Mvc.ModelBinding;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models
{
    public class DeliverySchedulingOrderModel
    {
        [NopResourceDisplayName("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDetails.DeliveryDate")]
        public string DeliveryTimeRange { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDetails.DeliveryTime")]
        public string DeliveryTime { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDetails.ShippingMethodName")]
        public string ShippingMethodName { get; set; }
    }
}
