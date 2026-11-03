using Nop.Services.Cms;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Components;

namespace Delsami.Plugin.Misc.Core
{
    public class CorePlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public CorePlugin(ILocalizationService localizationService, IPermissionService permissionService)
        {
            _localizationService = localizationService;
            _permissionService = permissionService;
        }

        #endregion

        #region Properties

        public bool HideInWidgetList => true;

        #endregion

        #region Methods

        public Type GetWidgetViewComponent(string widgetZone)
        {
            if (widgetZone == PublicWidgetZones.ContentAfter)
                return typeof(ToEnglishNumberViewComponent);
            else
                return null;
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>
           {
               PublicWidgetZones.ContentAfter
           });
        }

        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Delsami.Plugin.BaseName"] = "Delsami",
                ["Delsami.Plugin.SystemName"] = "Delsami",
                ["Delsami.Plugin.Settings.Saved"] = "Settings saved successfully",
                ["Delsami.Plugin.Settings.Error"] = "Error saving settings",
                ["Delsami.Plugin.Error"] = "Error saving settings",
                ["Delsami.Plugin.Misc.Core.Name"] = "Delsami Core",
                ["Delsami.Plugin.Misc.Core.SystemName"] = "DelsamiCore",
                ["Delsami.Plugin.Misc.Core.Configuration"] = "Configuration",
                ["Delsami.Plugin.Misc.Core.Configuration.SystemName"] = "CoreConfiguration",
                ["Delsami.Plugin.Misc.Core.Configuration.ManualGoldPrice"] = "ManualGoldPrice",
                ["Delsami.Plugin.Misc.Core.Configuration.UseManualGoldPrice"] = "UseManualGoldPrice",
                ["Delsami.Plugin.Misc.Core.Configuration.GoldPriceAdjustment"] = "GoldPriceAdjustment",
                ["Delsami.Plugin.Misc.Core.Configuration.NationalCodeAttributeId"] = "NationalCodeAttributeId",
                ["Delsami.Plugin.Misc.Core.Configuration.AllProductsCategoryId"] = "AllProductsCategoryId",
                ["Delsami.Plugin.Misc.Core.Configuration.Tax"] = "Tax",
                ["Delsami.Plugin.Misc.Core.Configuration.AllowGoldPriceAdjustment"] = "AllowGoldPriceAdjustment",
                ["Delsami.plugin.misc.sms.name "] = "Delsami SMS"

            });

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await _localizationService.DeleteLocaleResourcesAsync("Delsami.Plugin.Misc.Core");

            await base.UninstallAsync();
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return;
            }

            var baseName = await _localizationService.GetResourceAsync("Delsami.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("Delsami.Plugin.SystemName");
            var coreSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.Core.SystemName");
            var coreName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.Core.Name");
            var configurationName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.Core.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.Core.Configuration.SystemName");

            var delsamiSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (delsamiSiteMapNode == null)
                delsamiSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var coreChildNodes = new List<SiteMapNode>

             {
                 new SiteMapNode
                 {
                     Title = configurationName,
                     ControllerName = "Core",
                     ActionName = "Configure",
                     IconClass="fas fa-cog",
                     SystemName=configurationSystemName,
                     Visible = true
                 }
             };

            delsamiSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = coreName,
                SystemName = coreSystemName,
                Visible = true,
                IconClass = "fas fa-car",
                ChildNodes = coreChildNodes
            });


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(delsamiSiteMapNode);
        }

        #endregion
    }
}
