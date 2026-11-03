using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Delsami.Plugin.Widgets.AddressLocation.Areas.Admin.Models;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Widgets.AddressLocation.Areas.Admin.Controllers
{
    [AutoValidateAntiforgeryToken]
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class AddressLocationController : BasePluginController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly AddressLocationSettings _addressLocationSettings;
        private readonly IAddressService _addressService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerModelFactory _customerModelFactory;
        private readonly IAddressAttributeParser _addressAttributeParser;

        #endregion

        #region Ctor

        public AddressLocationController(IPermissionService permissionService,
                                         ILocalizationService localizationService,
                                         INotificationService notificationService,
                                         ISettingService settingService,
                                         IAddressAttributeService addressAttributeService,
                                         AddressLocationSettings addressLocationSettings,
                                         IAddressService addressService,
                                         ICustomerService customerService,
                                         ICustomerModelFactory customerModelFactory,
                                         IAddressAttributeParser addressAttributeParser)
        {
            _permissionService = permissionService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _settingService = settingService;
            _addressAttributeService = addressAttributeService;
            _addressLocationSettings = addressLocationSettings;
            _addressService = addressService;
            _customerService = customerService;
            _customerModelFactory = customerModelFactory;
            _addressAttributeParser = addressAttributeParser;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");

            ConfigurationModel model = new();

            model.AvailableAddressAttributes = (await _addressAttributeService.GetAllAddressAttributesAsync())
                                                                                            .Select(addAttr => new SelectListItem
                                                                                            {
                                                                                                Text = addAttr.Name,
                                                                                                Value = addAttr.Id.ToString(),
                                                                                            }).ToList();

            model.LatitudeAddressAttributeId = _addressLocationSettings.LatitudeAddressAttributeId;
            model.LongitudeAddressAttributeId = _addressLocationSettings.LongitudeAddressAttributeId;
            model.DigiKalaAddressAttributeId = _addressLocationSettings.DigiKalaAddressAttributeId;
            model.NeshanServiceApiKey = _addressLocationSettings.NeshanServiceApiKey;
            model.NeshanMapApiKey = _addressLocationSettings.NeshanMapApiKey;
            model.NeshanServiceTimeoutAsMilliseconds = _addressLocationSettings.NeshanServiceTimeoutAsMilliseconds;
            model.MapType = _addressLocationSettings.MapType;

            return View("~/Plugins/Delsami.Widgets.AddressLocation/Areas/Admin/Views/AddressLocation/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");

            if (ModelState.IsValid)
            {
                _addressLocationSettings.LatitudeAddressAttributeId = model.LatitudeAddressAttributeId;
                _addressLocationSettings.LongitudeAddressAttributeId = model.LongitudeAddressAttributeId;
                _addressLocationSettings.DigiKalaAddressAttributeId = model.DigiKalaAddressAttributeId;
                _addressLocationSettings.NeshanServiceApiKey = model.NeshanServiceApiKey;
                _addressLocationSettings.NeshanMapApiKey = model.NeshanMapApiKey;
                _addressLocationSettings.NeshanServiceTimeoutAsMilliseconds = model.NeshanServiceTimeoutAsMilliseconds;
                _addressLocationSettings.MapType = model.MapType;

                _settingService.SaveSetting(_addressLocationSettings);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.SettingsSavedSuccessfully"));

                return RedirectToAction("Configure", "AddressLocation");
            }

            model.AvailableAddressAttributes = (await _addressAttributeService.GetAllAddressAttributesAsync())
                                                                                            .Select(addAttr => new SelectListItem
                                                                                            {
                                                                                                Text = addAttr.Name,
                                                                                                Value = addAttr.Id.ToString(),
                                                                                            }).ToList();

            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Settings.Error"));

            return View("~/Plugins/Delsami.Widgets.AddressLocation/Areas/Admin/Views/AddressLocation/Configure.cshtml", model);
        }


        #endregion
    }
}
