using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Delsami.Plugin.Shipping.Core.Areas.Admin.Models.Cartons
{
    public record CartonModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Delsami.Plugin.Misc.ShippingCore.Admin.CartonModel.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ShippingCore.Admin.CartonModel.Width")]
        public decimal Width { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ShippingCore.Admin.CartonModel.Height")]
        public decimal Height { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ShippingCore.Admin.CartonModel.Length")]
        public decimal Length { get; set; } 
        
        [NopResourceDisplayName("Delsami.Plugin.Misc.ShippingCore.Admin.CartonModel.PostBoxNumber")]
        public int PostBoxNumber { get; set; }
    }
}
