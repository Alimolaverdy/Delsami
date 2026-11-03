using Microsoft.AspNetCore.Mvc.Rendering;
using Delsami.Plugin.Widgets.AddressLocation.Enums;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Delsami.Plugin.Widgets.AddressLocation.Areas.Admin.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public ConfigurationModel()
        {
            AvailableAddressAttributes = new();
        }

        [NopResourceDisplayName("Delsami.Plugin.Widgets.AddressLocation.Configuration.LatitudeAddressAttributeId")]
        public int LatitudeAddressAttributeId { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Widgets.AddressLocation.Configuration.LongitudeAddressAttributeId")]
        public int LongitudeAddressAttributeId { get; set; }

        public int DigiKalaAddressAttributeId { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Widgets.AddressLocation.Configuration.NeshanServiceApiKey")]
        public string NeshanServiceApiKey { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Widgets.AddressLocation.Configuration.NeshanMapApiKey")]
        public string NeshanMapApiKey { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Widgets.AddressLocation.Configuration.NeshanServiceTimeoutAsMilliseconds")]
        public int NeshanServiceTimeoutAsMilliseconds { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Widgets.AddressLocation.Configuration.MapType")]
        public MapType MapType { get; set; }

        public List<SelectListItem> AvailableAddressAttributes { get; set; }
    }
}
