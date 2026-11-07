using Delsami.Plugin.Shipping.DelsamiLogistic.Infrastructures.Permissions;
using Delsami.Plugin.Shipping.DelsamiLogistic.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.DelsamiLogistic
{
    public class DelsamiLogisticPlugin : BasePlugin, IAdminMenuPlugin, IShippingRateComputationMethod
    {
        #region Fields
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IDelsamiLogisticService _DelsamiLogistic;
        #endregion

        #region Ctor
        public DelsamiLogisticPlugin(ILocalizationService localizationService,
                                      IPermissionService permissionService,
                                      ISettingService settingService,
                                      IDelsamiLogisticService DelsamiLogistic)
        {
            _localizationService = localizationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _DelsamiLogistic = DelsamiLogistic;
        }
        #endregion

        #region Methods

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(DelsamiLogisticPermission.ManageDelsamiLogistic))
            {
                return;
            }
            var baseName = await _localizationService.GetResourceAsync("Delsami.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("Delsami.Plugin.SystemName");
            var DelsamiLogisticSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Shipping.DelsamiLogistic.SystemName");
            var DelsamiLogisticName = await _localizationService.GetResourceAsync("Delsami.Plugin.Shipping.DelsamiLogistic.Name");
            var configurationName = await _localizationService.GetResourceAsync("Delsami.Plugin.Shipping.DelsamiLogistic.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Shipping.DelsamiLogistic.Configuration.SystemName");

            var DelsamiSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (DelsamiSiteMapNode == null)
                DelsamiSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var DelsamiLogisticChildNodes = new List<SiteMapNode>
            {
                new SiteMapNode
                {
                    Title = configurationName,
                    ControllerName = "DelsamiLogistic",
                    ActionName = "Configure",
                    IconClass="fas fa-cog",
                    SystemName=configurationSystemName,
                    Visible = true,
                    RouteValues=new Microsoft.AspNetCore.Routing.RouteValueDictionary{ {"Area","Admin"} }

                }
            };

            DelsamiSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = DelsamiLogisticName,
                SystemName = DelsamiLogisticSystemName,
                Visible = true,
                IconClass = "fas fa-truck",
                ChildNodes = DelsamiLogisticChildNodes
            });


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(DelsamiSiteMapNode);

            await Task.CompletedTask;
        }

        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Delsami.Plugin.BaseName"] = "Delsami",
                ["Delsami.Plugin.SystemName"] = "Delsami",
                ["Delsami.Plugin.Shipping.DelsamiLogistic.SystemName"] = "DelsamiLogistic",
                ["Delsami.Plugin.Shipping.DelsamiLogistic.Name"] = "Delsami Logistic",
                ["Delsami.Plugin.Shipping.DelsamiLogistic.Configuration"] = "Configuration",
                ["Delsami.Plugin.Shipping.DelsamiLogistic.Configuration.SystemName"] = "DelsamiLogisticConfiguration",
                ["Delsami.Plugin.Shipping.DelsamiLogistic.Price"] = "NeighbouringFiveToNineKgPostPrice",
                ["Delsami.Plugins.Shipping.DelsamiLogistic.ShippingMethodId"] = " PostShippingMethod"

            });

            await _permissionService.InstallPermissionsAsync(new DelsamiLogisticPermission());

            if (_settingService.LoadSettingAsync<DelsamiLogisticSettings>() == null)
            {
                var setting = new DelsamiLogisticSettings { Price = 1_200_000, ShippingMethodId = 4 , PickupTruckPrice = 3_500_000_00 , VolumeMustCarryByPickupTruckInCM3 = 192_500 , WeightMustCarryByPickupTruckInGrams= 35_000 , OverSizeProductVolumeMustCarryByPickupTruckInCM3 = 70 };
                await _settingService.SaveSettingAsync(setting);
            }
            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }

        public async Task<decimal?> GetFixedRateAsync(GetShippingOptionRequest getShippingOptionRequest)
        {
            return await Task.FromResult<decimal?>(null);
        }

        public async Task<IShipmentTracker> GetShipmentTrackerAsync()
        {
            return await Task.FromResult<IShipmentTracker>(null);
        }

        public async Task<GetShippingOptionResponse> GetShippingOptionsAsync(GetShippingOptionRequest getShippingOptionRequest)
        {
            var shippingOptionResponse = new GetShippingOptionResponse()
            {
                ShippingOptions = await _DelsamiLogistic.PrepareShippingOptionsAsync(getShippingOptionRequest)
            };
            return shippingOptionResponse;
        }

        #endregion
    }
}
