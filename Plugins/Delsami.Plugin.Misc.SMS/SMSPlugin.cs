using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.SMS.Infrastructure.Permissions;

namespace Delsami.Plugin.Misc.SMS
{
    public class SMSPlugin : BasePlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;

        public SMSPlugin(IWebHelper webHelper,
                         ILocalizationService localizationService,
                         ISettingService settingService,
                         IPermissionService permissionService)
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
            _settingService = settingService;
            _permissionService = permissionService;
        }

        #endregion

        #region Ctor

        #endregion

        #region Methods

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/SMS/Configure";
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return;
            }
            var baseName = await _localizationService.GetResourceAsync("Delsami.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("Delsami.Plugin.SystemName");
            var smsSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.SMS.SystemName");
            var smsName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.SMS.Name");
            var configurationName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.SMS.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.SMS.Configuration.SystemName");
            var queuedSMSListName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.SMS.QueuedSMS.List");
            var queuedSMSListSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.SMS.QueuedSMS.List.SystemName");

            var delsamiSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (delsamiSiteMapNode == null)
                delsamiSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var carInfoChildNodes = new List<SiteMapNode>
            {
                new SiteMapNode
                {
                    Title = configurationName,
                    ControllerName = "SMS",
                    ActionName = "Configure",
                    IconClass="fas fa-cog",
                    SystemName=configurationSystemName,
                    Visible = true,
                    RouteValues=new Microsoft.AspNetCore.Routing.RouteValueDictionary{ {"Area","Admin"} }

                }
            };

            delsamiSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = smsName,
                SystemName = smsSystemName,
                Visible = true,
                IconClass = "fas fa-sms",
                ChildNodes = carInfoChildNodes
            });


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(delsamiSiteMapNode);

            await Task.CompletedTask;
        }

        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Delsami.Plugin.BaseName"] = "Delsami",
                ["Delsami.Plugin.SystemName"] = "Delsami",
                ["Delsami.Plugin.Misc.SMS.SystemName"] = "SMS",
                ["Delsami.Plugin.Misc.SMS.Name"] = "SMS",
                ["Delsami.Plugin.Settings.Saved"] = "Settings saved successfully",
                ["Delsami.Plugin.Settings.Error"] = "Error saving settings",
                ["Delsami.Plugin.Error"] = "Error saving settings",
                ["Delsami.Plugin.Misc.SMS.Configuration"] = "Configuration",
                ["Delsami.Plugin.Misc.SMS.Configuration.SystemName"] = "SMSConfiguration",
                ["Delsami.Plugin.Misc.SMS.QueuedSMS.List.SystemName"] = "QueuedSMSList",
                ["Delsami.Plugin.Misc.SMS.QueuedSMS.List"] = "Queued SMSs",
                ["Delsami.Plugin.Misc.SMS.QueuedSMSs.Fields.SenderNumber"] = "Sender number",
                ["Delsami.Plugin.Misc.SMS.QueuedSMSs.Fields.ReceiverNumber"] = "Receiver number",
                ["Delsami.Plugin.Misc.SMS.QueuedSMSs.Fields.Body"] = "Message body",
                ["Delsami.Plugin.Misc.SMS.QueuedSMSs.Fields.CreatedDateTimeStr"] = "Created date",
                ["Delsami.Plugin.Misc.SMS.QueuedSMSs.Fields.SentDateTimeStr"] = "Sent date",
                ["Delsami.Plugin.Misc.SMS.QueuedSMSs.Fields.SentTries"] = "Sent tries",
                ["Delsami.Plugin.Misc.SMS.Configuration.ApiKey"] = "ApiKey",
                ["Delsami.Plugin.Misc.SMS.Configuration.SenderNumber"] = "Number Of Sender",
                ["Delsami.Plugin.Misc.SMS.Configuration.MaximumLengthOfSMS"] = "Maximum Length Of SMS",
                ["Delsami.Plugin.Misc.SMS.Configuration.ExchangeName"] = "Exchange Name",
                ["Delsami.Plugin.Misc.SMS.Configuration.RoutingKey"] = "Routing Key",
                ["Delsami.Plugin.Misc.SMS.Configuration.Password"] = "Password Of SMS Service",

            });

            await _permissionService.InstallPermissionsAsync(new SMSPermission());

            var smsSettings = new SMSSettings
            {
                MaximumLengthOfSMS = 300
            };

            await _settingService.SaveSettingAsync(smsSettings);

            await base.InstallAsync();
        }

        #endregion
    }
}