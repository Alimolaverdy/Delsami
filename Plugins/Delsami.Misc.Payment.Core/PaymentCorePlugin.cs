using Delsami.Plugin.Payment.Core.Areas.Admin.Components;
using Delsami.Plugin.Payment.Core.Infrastructure.Permissions;
using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payment.Core
{
    public class PaymentCorePlugin : BasePlugin, IAdminMenuPlugin, IWidgetPlugin
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public PaymentCorePlugin(IWebHelper webHelper,
                                 ILocalizationService localizationService,
                                 ISettingService settingService,
                                 IPermissionService permissionService)
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
            _settingService = settingService;
            _permissionService = permissionService;
        }

        public bool HideInWidgetList => true;

        #endregion

        #region Methods

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/DelsamiPayment/Configure";
        }

        public Type GetWidgetViewComponent(string widgetZone)
        {
            if (widgetZone == AdminWidgetZones.OrderDetailsBlock)
                return typeof(PaymentsForOrderViewComponent);

            return null;
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            IList<string> widgets = new List<string> { AdminWidgetZones.OrderDetailsBlock };

            return Task.FromResult(widgets);
        }

        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Delsami.Plugin.BaseName"] = "Delsami",
                ["Delsami.Plugin.SystemName"] = "Delsam",
                ["Delsami.Plugin.Settings.Saved"] = "Settings saved successfully",
                ["Delsami.Plugin.Settings.Error"] = "Error saving settings",
                ["Delsami.Plugin.Error"] = "Error saving settings",
                ["Delsami.Plugin.Payment.Core.Name"] = "Delsami Payment",
                ["Delsami.Plugin.Payment.Core.SystemName"] = "PaymentCore",
                ["Delsami.Plugin.Payment.Core.Configuration"] = "Configuration",
                ["Delsami.Plugin.Payment.Core.Configuration.SystemName"] = "materialConfiguration",
                ["Delsami.Plugin.Payment.Core.PaymentStatus.Completed"] = "Completed",
                ["Delsami.Plugin.Payment.Core.Configuration.InProgress"] = "InProgress",
                ["Delsami.Plugin.Payment.Core.Configuration.Failed"] = "Failed",
                ["Delsami.Plugin.Payment.Core.PaymentStatus.Verified"] = "Verified",
                ["Delsami.Plugin.Payment.Core.PaymentStatus.Initial"] = "Initial",
                ["Delsami.Plugin.Payment.Core.PaymentStatus.InProgress"] = "InProgress",
                ["Delsami.Plugin.Payment.Core.PaymentStatus.Failed"] = "Failed",
                ["Delsami.Plugin.Payment.Core.Payments"] = "Payments",
                ["Delsami.Plugin.Payment.Core.Payment.List"] = "Payment List",
                ["Delsami.Plugin.Payment.Core.PaymentGateway.Pasargad"] = "Pasargad",
                ["Delsami.Plugin.Payment.Core.Admin.PaymentModel.Id"] = "Payment Id",
                ["Delsami.Plugin.Payment.Core.Admin.PaymentModel.OrderId"] = "Order Id",
                ["Delsami.Plugin.Payment.Core.Admin.PaymentModel.Gateway"] = "Gateway",
                ["Delsami.Plugin.Payment.Core.Admin.PaymentModel.TeminalNo"] = "Terminal No",
                ["Delsami.Plugin.Payment.Core.Admin.PaymentModel.ReferenceNumber"] = "Reference Number",
                ["Delsami.Plugin.Payment.Core.Admin.PaymentModel.TrackId"] = "Track Id",
                ["Delsami.Plugin.Payment.Core.Admin.PaymentModel.MaskedCardNumber"] = "Masked Card Number",
                ["Delsami.Plugin.Payment.Core.Admin.PaymentModel.HashedCardNumber"] = "Hashed Card Number",
                ["Delsami.Plugin.Payment.Core.Admin.PaymentModel.UrlId"] = "Url Id",
                ["Delsami.Plugin.Payment.Core.Admin.PaymentModel.Amount"] = "Amount",
                ["Delsami.Plugin.Payment.Core.Admin.PaymentModel.AdditionalData"] = "Additional Data",
                ["Delsami.Plugin.Payment.Core.Admin.PaymentModel.CreatedDateTime"] = "Created On",
                ["Delsami.Plugin.Payment.Core.Admin.PaymentModel.VerifyDateTime"] = "Verified On",
                ["Delsami.Plugin.Payment.Core.Admin.PaymentModel.ConfirmDateTime"] = "Confirmed On",
                ["Delsami.Plugin.Payment.Core.Admin.PaymentModel.Status"] = "Status",
                ["Delsami.Plugin.Payment.Core.Admin.PaymentModel.ResultCode"] = "Result Code",
                ["Delsami.Plugin.Payment.Core.Admin.PaymentModel.Completed"] = "Completed",
            });

            await _permissionService.InstallPermissionsAsync(new PaymentCorePermission());

            await base.InstallAsync();
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(PaymentCorePermission.ManagePayment))
            {
                return;
            }
            var baseName = await _localizationService.GetResourceAsync("Delsami.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("Delsami.Plugin.SystemName");
            var paymentCoreSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Payment.Core.SystemName");
            var paymentCoreName = await _localizationService.GetResourceAsync("Delsami.Plugin.Payment.Core.Name");
            var configurationName = await _localizationService.GetResourceAsync("Delsami.Plugin.Payment.Core.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Payment.Core.Configuration.SystemName");
            var paymentListName = await _localizationService.GetResourceAsync("Delsami.Plugin.Payment.Core.Payment.List");
            var paymentListSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Payment.Core.Payment.List.SystemName");

            var delsamiSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (delsamiSiteMapNode == null)
                delsamiSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var paymentCoreChildNodes = new List<SiteMapNode>
            {
                //new SiteMapNode
                //{
                //    Title = configurationName,
                //    ControllerName = "PaymentCore",
                //    ActionName = "Configure",
                //    IconClass="fas fa-cog",
                //    SystemName=configurationSystemName,
                //    Visible = true
                //},
                 new SiteMapNode
                {
                    Title = paymentListName,
                    ControllerName = "Payment",
                    ActionName = "List",
                    IconClass="fas fa-cog",
                    SystemName=paymentListSystemName,
                    Visible = true
                },
            };

            delsamiSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = paymentCoreName,
                SystemName = paymentCoreSystemName,
                Visible = true,
                IconClass = "fas fa-car",
                ChildNodes = paymentCoreChildNodes
            });


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(delsamiSiteMapNode);

            await Task.CompletedTask;
        }

        public override async Task UninstallAsync()
        {
            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Delsami.Plugin.Payment.Core");

            await base.UninstallAsync();

        }

        #endregion
    }
}
