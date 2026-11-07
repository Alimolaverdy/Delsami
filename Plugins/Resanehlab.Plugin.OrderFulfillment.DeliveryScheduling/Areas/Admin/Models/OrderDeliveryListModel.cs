using Nop.Web.Framework.Models;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models
{
    public record OrderDeliveryListModel : BasePagedListModel<OrderDeliveryModel>
    {
    }
}
