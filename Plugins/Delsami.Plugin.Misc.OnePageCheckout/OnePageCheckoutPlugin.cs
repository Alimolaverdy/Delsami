using Nop.Core;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ExtendedVendor
{
    public class OnePageCheckoutPlugin : BasePlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public OnePageCheckoutPlugin(IWebHelper webHelper,
            ILocalizationService localizationService,
            IPermissionService permissionService)
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
            _permissionService = permissionService;
        }

        #endregion

        #region Methods

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/OnePageCheckout/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Delsami.Plugin.BaseName"] = "Delsami",
                ["Delsami.Plugin.SystemName"] = "Delsami",
                ["Delsami.Plugin.Settings.Saved"] = "Settings saved successfully",
                ["Delsami.Plugin.Settings.Error"] = "Error saving settings",
                ["Delsami.Plugin.Error"] = "Error saving settings",
                ["Delsami.Plugin.Misc.OnePageCheckout.Name"] = "One page checkout",
                ["Delsami.Plugin.Misc.OnePageCheckout.SystemName"] = "OnePageCheckout",
                ["Delsami.Plugin.Misc.OnePageCheckout.Configuration"] = "Configuration",
                ["Delsami.Plugin.Misc.OnePageCheckout.Configuration.SystemName"] = "OnePageCheckoutConfiguration",
                ["Delsami.Plugin.Misc.OnePageCheckout.AddNewAddress"] = "Add new address",
                ["Delsami.Plugin.Misc.OnePageCheckout.CityRequired"] = "City is required",

            });

            await base.InstallAsync();
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return;
            }
            var baseName = await _localizationService.GetResourceAsync("Delsami.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("Delsami.Plugin.SystemName");
            var onePageCheckoutSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.OnePageCheckout.SystemName");
            var onePageCheckoutName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.OnePageCheckout.Name");
            var configurationName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.OnePageCheckout.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.OnePageCheckout.Configuration.SystemName");

            var DelsamiSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (DelsamiSiteMapNode == null)
                DelsamiSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var extendedVendorChildNodes = new List<SiteMapNode>
            {
                new SiteMapNode
                {
                    Title = configurationName,
                    ControllerName = "OnePageCheckout",
                    ActionName = "Configure",
                    IconClass="fas fa-cog",
                    SystemName=configurationSystemName,
                    Visible = true
                },
            };

            DelsamiSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = onePageCheckoutName,
                SystemName = onePageCheckoutSystemName,
                Visible = true,
                IconClass = "fas fa-car",
                ChildNodes = extendedVendorChildNodes
            });


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(DelsamiSiteMapNode);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Delsami.Plugin.Misc.OnePageCheckout");

            await base.UninstallAsync();

        }

        #endregion
    }
}
