using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.News;
using NopShop.Plugin.Misc.LoginWithMobile.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Seo;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NopShop.Plugin.Misc.LoginWithMobile.Controllers
{
    public class LoginWithMobileController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IWebHelper _webHelper;
        private readonly LoginWithMobileSetting _loginWithMobileSetting;

        #endregion

        #region Ctor

        public LoginWithMobileController(ILocalizationService localizationService,
            INotificationService notificationService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWorkContext workContext,
            IStaticCacheManager cacheManager,
            IWebHelper webHelper,
            LoginWithMobileSetting loginWithMobileSetting)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _settingService = settingService;
            _storeContext = storeContext;
            _workContext = workContext;
            _cacheManager = cacheManager;
            _webHelper = webHelper;
            _loginWithMobileSetting = loginWithMobileSetting;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            var storeId = _storeContext.GetActiveStoreScopeConfigurationAsync();

            var model = new ConfigurationModel();

            model.Enable = _loginWithMobileSetting.Enable;
            model.EnableLoginWithEmail = _loginWithMobileSetting.EnableLoginWithEmail;
            model.AutoGenerateEmail = _loginWithMobileSetting.AutoGenerateEmail;
            model.StoreUrl = _loginWithMobileSetting.StoreUrl;
            model.RedirectToCustomerInfo = _loginWithMobileSetting.RedirectToCustomerInfo;
            model.SmsToken = _loginWithMobileSetting.SmsToken;
            model.UseDedicatedLine = _loginWithMobileSetting.UseDedicatedLine;
            model.SharedTemplate = _loginWithMobileSetting.SharedTemplate;
            model.SmsStaticNumber = _loginWithMobileSetting.SmsStaticNumber;
            model.MessageTemplate = _loginWithMobileSetting.MessageTemplate;
            model.ShowCaptchaOnLoginWithMobilePage = _loginWithMobileSetting.ShowCaptchaOnLoginWithMobilePage;
            model.ShowCaptchaOnActivationCodePage = _loginWithMobileSetting.ShowCaptchaOnActivationCodePage;
            model.ShowCaptchaOnChangePhoenumberPage = _loginWithMobileSetting.ShowCaptchaOnChangePhoenumberPage;
            model.ShowCaptchaOnChangePhoenumberActivationCodePage = _loginWithMobileSetting.ShowCaptchaOnChangePhoenumberActivationCodePage;

            model.ActivationCodeValidityPeriod = _loginWithMobileSetting.ActivationCodeValidityPeriod;
            model.ActivationCodeCountOfSend = _loginWithMobileSetting.ActivationCodeCountOfSend;

            model.LoginDescription = _loginWithMobileSetting.LoginDescription;

            model.SmsProviderId = _loginWithMobileSetting.SmsProviderId;

            model.FaraPayamakUsername = _loginWithMobileSetting.FaraPayamakUsername;
            model.FaraPayamakPassword = _loginWithMobileSetting.FaraPayamakPassword;
            model.FaraPayamakTemplateId = _loginWithMobileSetting.FaraPayamakTemplateId;
            model.FaraPayamakUseDedicatedLine = _loginWithMobileSetting.FaraPayamakUseDedicatedLine;
            model.FaraPayamakDedicatedLineNumber = _loginWithMobileSetting.FaraPayamakDedicatedLineNumber;
            model.FaraPayamakTemplateMessage = _loginWithMobileSetting.FaraPayamakTemplateMessage;

            model.AvailableSmsProviders.Add(new SelectListItem()
            {
                Text = "کاوه نگار",
                Value = "1"
            });

            model.AvailableSmsProviders.Add(new SelectListItem()
            {
                Text = "فرا پیامک",
                Value = "2"
            });

            model.EnableLoginWithOneToFive = _loginWithMobileSetting.EnableLoginWithOneToFive;

            return View("~/Plugins/NopShop.Misc.LoginWithMobile/Views/Configure.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public async Task<IActionResult> ConfigureAsync(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            _loginWithMobileSetting.Enable = model.Enable;
            _loginWithMobileSetting.EnableLoginWithEmail = model.EnableLoginWithEmail;
            _loginWithMobileSetting.AutoGenerateEmail = model.AutoGenerateEmail;
            _loginWithMobileSetting.StoreUrl = model.StoreUrl;
            _loginWithMobileSetting.RedirectToCustomerInfo = model.RedirectToCustomerInfo;
            _loginWithMobileSetting.SmsToken = model.SmsToken;
            _loginWithMobileSetting.UseDedicatedLine = model.UseDedicatedLine;
            _loginWithMobileSetting.SharedTemplate = model.SharedTemplate;
            _loginWithMobileSetting.SmsStaticNumber = model.SmsStaticNumber;
            _loginWithMobileSetting.MessageTemplate = model.MessageTemplate;
            _loginWithMobileSetting.ShowCaptchaOnLoginWithMobilePage = model.ShowCaptchaOnLoginWithMobilePage;
            _loginWithMobileSetting.ShowCaptchaOnActivationCodePage = model.ShowCaptchaOnActivationCodePage;
            _loginWithMobileSetting.ShowCaptchaOnChangePhoenumberPage = model.ShowCaptchaOnChangePhoenumberPage;
            _loginWithMobileSetting.ShowCaptchaOnChangePhoenumberActivationCodePage = model.ShowCaptchaOnChangePhoenumberActivationCodePage;
            _loginWithMobileSetting.ActivationCodeValidityPeriod = model.ActivationCodeValidityPeriod;
            _loginWithMobileSetting.ActivationCodeCountOfSend = model.ActivationCodeCountOfSend;
            _loginWithMobileSetting.LoginDescription = model.LoginDescription;
            _loginWithMobileSetting.SmsProviderId = model.SmsProviderId;
            _loginWithMobileSetting.FaraPayamakUsername = model.FaraPayamakUsername;
            _loginWithMobileSetting.FaraPayamakPassword = model.FaraPayamakPassword;
            _loginWithMobileSetting.FaraPayamakTemplateId = model.FaraPayamakTemplateId;
            _loginWithMobileSetting.FaraPayamakUseDedicatedLine = model.FaraPayamakUseDedicatedLine;
            _loginWithMobileSetting.FaraPayamakDedicatedLineNumber = model.FaraPayamakDedicatedLineNumber;
            _loginWithMobileSetting.FaraPayamakTemplateMessage = model.FaraPayamakTemplateMessage;
            _loginWithMobileSetting.EnableLoginWithOneToFive = model.EnableLoginWithOneToFive;

            await _settingService.SaveSettingAsync(_loginWithMobileSetting);

            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return RedirectToAction("Configure");
        }

        #endregion
    }
}