using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Delsami.Plugin.Misc.SMS.Areas.Admin.Models;
using System.Threading.Tasks;
using Nop.Services.Messages;
using Nop.Services.Localization;
using Delsami.Plugin.Misc.SMS.Infrastructure.Permissions;

namespace Delsami.Plugin.Misc.SMS.Areas.Admin.Controllers
{

    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class SMSController : BasePluginController
    {
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;

        public SMSController(IPermissionService permissionService,
                             ISettingService settingService,
                             INotificationService notificationService,
                             ILocalizationService localizationService)
        {
            _permissionService = permissionService;
            _settingService = settingService;
            _notificationService = notificationService;
            _localizationService = localizationService;
        }

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(SMSPermission.ManageSMS))
                return Content("Access denied");


            var smsSettings = await _settingService.LoadSettingAsync<SMSSettings>();

            ConfigurationModel model = new();

            model.SenderNumber = smsSettings.SenderNumber;   
            model.MaximumLengthOfSMS = smsSettings.MaximumLengthOfSMS;
            model.Apikey = smsSettings.Apikey;
            model.BaseUrl = smsSettings.BaseUrl;

            return View("~/Plugins/Delsami.Misc.SMS/Areas/Admin/Views/SMS/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            var smsSettings = await _settingService.LoadSettingAsync<SMSSettings>();

            smsSettings.SenderNumber = model.SenderNumber;
            smsSettings.MaximumLengthOfSMS = model.MaximumLengthOfSMS;
            smsSettings.Apikey = model.Apikey;
            smsSettings.BaseUrl = model.BaseUrl;

            await _settingService.SaveSettingAsync(smsSettings);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

            return RedirectToAction("Configure");
        }
    }
}
