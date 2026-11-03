using Nop.Web.Framework.Models;

namespace Delsami.Plugin.Misc.OnePageCheckout.Models;

public record DelsamiCheckoutCompletedModel : BaseNopModel
{
    public int OrderId { get; set; }
    public string CustomOrderNumber { get; set; }
    public bool OnePageCheckoutEnabled { get; set; }
    public string ShippingDescription { get; set; }
}
