using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Delsami.Plugin.Misc.Core.Areas.Admin.Models;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Areas.Admin.Controllers
{

    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class CoreController : BasePluginController
    {

        #region Fields

        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public CoreController(ISettingService settingService,
                      INotificationService notificationService,
                      ILocalizationService localizationService)
        {
            _settingService = settingService;
            _notificationService = notificationService;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure()
        {
            var coreSettings = await _settingService.LoadSettingAsync<CoreSettings>();

            var model = new ConfigurationModel
            {
				NationalCodeAttributeId = coreSettings.NationalCodeAttributeId,
                AllProductsCategoryId = coreSettings.AllProductsCategoryId,
            };


            return View("~/Plugins/Delsami.Misc.Core/Areas/Admin/Views/Core/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            var coreSettings = await _settingService.LoadSettingAsync<CoreSettings>();

            coreSettings.NationalCodeAttributeId = model.NationalCodeAttributeId;
            coreSettings.AllProductsCategoryId = model.AllProductsCategoryId;

            await _settingService.SaveSettingAsync(coreSettings);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

            return RedirectToAction("Configure");
        }

        #endregion

    }
}
