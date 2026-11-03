using Nop.Web.Models.Common;

namespace Delsami.Plugin.Misc.OnePageCheckout.Models
{
    public record DelsamiAddressModel : AddressModel
    {
        public int CityId { get; set; }
    }
}
