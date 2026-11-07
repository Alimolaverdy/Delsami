using Nop.Core.Domain.Messages;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Delsami.Plugin.Shipping.Core
{
    public class ShippingCorePlugin : BasePlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public ShippingCorePlugin(IWebHelper webHelper,
                                  ISettingService settingService,
                                  ILocalizationService localizationService,
                                  IMessageTemplateService messageTemplateService,
                                  EmailAccountSettings emailAccountSettings,
                                  IScheduleTaskService scheduleTaskService,
                                  IPermissionService permissionService)
        {
            _webHelper = webHelper;
            _settingService = settingService;
            _localizationService = localizationService;
            _messageTemplateService = messageTemplateService;
            _emailAccountSettings = emailAccountSettings;
            _scheduleTaskService = scheduleTaskService;
            _permissionService = permissionService;
        }

        #endregion

        #region Methods

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
                ["Delsami.Plugin.Settings.Error"] = "Error in saving settings",
                ["Delsami.Plugin.Error"] = "Error saving settings",
                ["Delsami.Plugin.Misc.ShippingCore.Name"] = "Shipping Core",
                ["Delsami.Plugin.Misc.ShippingCore.SystemName"] = "Shipping Core",
                ["Delsami.Plugin.Misc.ShippingCore.Admin.Carton.List"] = "Carton management",
                ["Delsami.Plugin.Misc.ShippingCore.Admin.Carton.List.SystemName"] = "CartonManagementSystemName",
                ["Delsami.Plugin.Misc.ShippingCore.Admin.CartonModel.Name"] = "Name",
                ["Delsami.Plugin.Misc.ShippingCore.Admin.CartonModel.Width"] = "Width",
                ["Delsami.Plugin.Misc.ShippingCore.Admin.CartonModel.Height"] = "Height",
                ["Delsami.Plugin.Misc.ShippingCore.Admin.CartonModel.Length"] = "Length",
                ["Delsami.Plugin.Misc.ShippingCore.Admin.Carton.AddNew"] = "Add new carton",
                ["Delsami.Plugin.Misc.ShippingCore.Admin.Carton.Edit"] = "Edit carton",
                ["Delsami.Plugin.Misc.ShippingCore.Admin.Carton.BackToList"] = "Back to list",
                ["Delsami.Plugin.Misc.ShippingCore.Message.Carton.Added"] = "Carton added successfully",
                ["Delsami.Plugin.Misc.ShippingCore.Message.Carton.Updated"] = "Carton updated successfully",
                ["Delsami.Plugin.Misc.ShippingCore.Message.Carton.Deleted"] = "Carton deleted successfully",

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
            var shippingCoreSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShippingCore.SystemName");
            var shippingCoreName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShippingCore.Name");

            var cartonManagementName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShippingCore.Admin.Carton.List");
            var cartonManagementSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShippingCore.Admin.Carton.List.SystemName");

            var logisticsScheduleName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShippingCore.Admin.LogisticsSchedule.List");
            var logisticsScheduleSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShippingCore.Admin.LogisticsSchedule.List.SystemName");

            var delsamiSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (delsamiSiteMapNode == null)
                delsamiSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var shippingCoreChildNodes = new List<SiteMapNode>
            {
                new SiteMapNode
                {
                    Title = cartonManagementName,
                    ControllerName = "Carton",
                    ActionName = "List",
                    IconClass="far fa-dot-circle",
                    SystemName=cartonManagementSystemName,
                    Visible = true
                },

                new SiteMapNode
                {
                    Title = logisticsScheduleName,
                    ControllerName = "LogisticsSchedule",
                    ActionName = "List",
                    IconClass="far fa-dot-circle",
                    SystemName=logisticsScheduleSystemName,
                    Visible = true
                },
            };

            delsamiSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = shippingCoreName,
                SystemName = shippingCoreSystemName,
                Visible = true,
                IconClass = "fas fa-car",
                ChildNodes = shippingCoreChildNodes
            });


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(delsamiSiteMapNode);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //settings
            //await _settingService.DeleteSettingAsync<ShipmentManagementSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Delsami.Plugin.Misc.ShippingCore");

            await base.UninstallAsync();
        }

        #endregion
    }
}
