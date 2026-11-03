using Nop.Web.Framework.Models;
using Nop.Web.Models.Customer;

namespace Delsami.Plugin.Misc.OnePageCheckout.Models
{
    public record DelsamiCustomerAddressEditModel : BaseNopModel
    {
        public DelsamiAddressModel Address { get; set; }
    }
}
