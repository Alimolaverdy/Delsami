using Nop.Web.Framework.Models;

namespace Delsami.Plugin.Widgets.AddressLocation.Models.Addresses
{
    public record DelsamiCustomerAddressEditModel : BaseNopModel
    {
        public DelsamiCustomerAddressEditModel()
        {
            Address = new();
        }

        public DelsamiAddressModel Address { get; set; }
    }
}
