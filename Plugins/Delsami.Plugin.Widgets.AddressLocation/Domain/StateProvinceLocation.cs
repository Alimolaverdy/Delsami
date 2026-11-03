using Nop.Core;

namespace Delsami.Plugin.Widgets.AddressLocation.Domain
{
    public class StateProvinceLocation : BaseEntity
    {
        public int StateProvinceId{ get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
