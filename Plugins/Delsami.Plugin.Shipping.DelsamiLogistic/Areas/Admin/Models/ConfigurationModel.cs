using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Delsami.Plugin.Shipping.DelsamiLogistic.Areas.Admin.Models
{
    public class ConfigurationModel
    {
        public ConfigurationModel()
        {
            AvailableShippingMethods = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Delsami.Plugins.Shipping.DelsamiLogistic.ShippingMethodId")]
        public int ShippingMethodId { get; set; }

        [NopResourceDisplayName("Delsami.Plugins.Shipping.DelsamiLogistic.PickupTrucShippingMethodId")]
        public int PickupTruckShippingMethodId { get; set; }

        [NopResourceDisplayName("Delsami.Plugins.Shipping.DelsamiLogistic.ScheduledShippingMethodId")]
        public int ScheduledShippingMethodId { get; set; }

        [NopResourceDisplayName("Delsami.Plugins.Shipping.DelsamiLogistic.ScheduledShippingMethodWithTruckId")]
        public int ScheduledShippingMethodWithTruckId { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Shipping.DelsamiLogistic.Price")]
        public decimal Price { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Shipping.DelsamiLogistic.PickupTruckPrice")]
        public decimal PickupTruckPrice { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Shipping.DelsamiLogistic.ScheduledPrice")]
        public decimal ScheduledPrice { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Shipping.DelsamiLogistic.ScheduledPriceForHeavyOrBigBasket")]
        public decimal ScheduledPriceForHeavyOrBigBasket { get; set; }

        [NopResourceDisplayName("Delsami.Plugins.Shipping.DelsamiLogistic.ApiBaseUrl")]
        public string ApiBaseUrl { get; set; }

        [NopResourceDisplayName("Delsami.Plugins.Shipping.DelsamiLogistic.ApiToken")]
        public string ApiToken { get; set; }

        public IList<SelectListItem> AvailableShippingMethods { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Shipping.DelsamiLogistic.WeightMustCarryByPickupTruckInGrams")]
        public decimal WeightMustCarryByPickupTruckInGrams { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Shipping.DelsamiLogistic.volumeMustCarryByPickupTruckInCM3")]
        public decimal VolumeMustCarryByPickupTruckInCM3 { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Shipping.DelsamiLogistic.OverSizeProductVolumeMustCarryByPickupTruckInCM3")]
        public decimal OverSizeProductVolumeMustCarryByPickupTruckInCM3 { get; set; }


    }
}
