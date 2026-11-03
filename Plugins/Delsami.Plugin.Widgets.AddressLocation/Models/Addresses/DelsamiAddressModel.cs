using Nop.Web.Models.Common;

namespace Delsami.Plugin.Widgets.AddressLocation.Models.Addresses
{
    public record DelsamiAddressModel : AddressModel
    {
        public int CityId { get; set; }
    }
}
