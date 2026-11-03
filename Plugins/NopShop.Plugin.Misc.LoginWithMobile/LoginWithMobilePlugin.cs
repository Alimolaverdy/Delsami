using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using NopShop.Plugin.Misc.LoginWithMobile.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NopShop.Plugin.Misc.LoginWithMobile
{
    /// <summary>
    /// Represents the SendinBlue plugin
    /// </summary>
    public class LoginWithMobilePlugin : BasePlugin, IMiscPlugin, IWidgetPlugin
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly LoginWithMobileSetting _loginWithMobileSetting;
        private readonly ILocalizationService _localizationService;


        #endregion

        #region Ctor

        public LoginWithMobilePlugin(IWebHelper webHelper,
            ISettingService settingService,
            LoginWithMobileSetting loginWithMobileSetting,
            ILocalizationService localizationService)
        {
            _webHelper = webHelper;
            _settingService = settingService;
            _loginWithMobileSetting = loginWithMobileSetting;
            _localizationService = localizationService;
        }

        public bool HideInWidgetList => false;

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/LoginWithMobile/Configure";
        }

        public Type GetWidgetViewComponent(string widgetZone)
        {
            var loginWithMobileSetting = EngineContext.Current.Resolve<LoginWithMobileSetting>();
            if (loginWithMobileSetting.Enable)
            {
                if (widgetZone == PublicWidgetZones.AccountNavigationAfter)
                    return typeof(WidgetsChangePhonenumberDataViewComponent);

                else if (widgetZone == "lwe-mobile-nav-link")
                    return typeof(WidgetsAditionalMobileNavLinksDataViewComponent);
            }

            return typeof(Type);
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            var loginWithMobileSetting = EngineContext.Current.Resolve<LoginWithMobileSetting>();
            if (loginWithMobileSetting.Enable)
            {
                return Task.FromResult<IList<string>>(new List<string> {
                    PublicWidgetZones.AccountNavigationAfter,
                    "lwe-mobile-nav-link"
                });
            }
            else
            {
                return Task.FromResult<IList<string>>(new List<string> {});
            }
        }





        /// <summary>
        /// Install the plugin
        /// </summary>
        public override async Task InstallAsync()
        {

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["ZarIran.Plugin.Misc.MarketerManagement.SendSMSTemplate"] = "Sms Send To"
            });

            await _settingService.DeleteSettingAsync<LoginWithMobileSetting>();

            var loginWithMobileSetting = new LoginWithMobileSetting();

            loginWithMobileSetting.EnableLoginWithEmail = true;
            loginWithMobileSetting.AutoGenerateEmail = true;
            loginWithMobileSetting.SharedTemplate = "OTPVerify";
            loginWithMobileSetting.MessageTemplate = "کد فعال سازی: {0}";
            loginWithMobileSetting.ActivationCodeCountOfSend = 5;
            loginWithMobileSetting.ActivationCodeValidityPeriod = 5;
            loginWithMobileSetting.Enable = true;

            await _settingService.SaveSettingAsync(loginWithMobileSetting);

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override async Task UninstallAsync()
        {
            await _settingService.DeleteSettingAsync<LoginWithMobileSetting>();

            await base.UninstallAsync();
        }

        #endregion
    }
}