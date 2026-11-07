using Microsoft.AspNetCore.Mvc;
using Delsami.Plugin.Shipping.DelsamiLogistic.Infrastructures.Permissions;
using Nop.Services.Configuration;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Controllers;
using System.Threading.Tasks;
using Delsami.Plugin.Shipping.DelsamiLogistic.Areas.Admin.Models;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Delsami.Plugin.Shipping.DelsamiLogistic.Areas.Admin.Controllers
{
    public class DelsamiLogisticController : BaseAdminController
    {
        #region Fields
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IShippingService _shippingService;
        #endregion

        #region Ctor

        public DelsamiLogisticController(IPermissionService permissionService,
                                          ISettingService settingService,
                                          ILocalizationService localizationService,
                                          INotificationService notificationService,
                                          IShippingService shippingService)
        {
            _permissionService = permissionService;
            _settingService = settingService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _shippingService = shippingService;
        }

        #endregion

        #region Methods
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(DelsamiLogisticPermission.ManageDelsamiLogistic))
                return await AccessDeniedDataTablesJson();

            var settings = await _settingService.LoadSettingAsync<DelsamiLogisticSettings>();
            var model = new ConfigurationModel
            {
                ShippingMethodId = settings.ShippingMethodId,
                PickupTruckShippingMethodId = settings.PickupTruckShippingMethodId,
                ScheduledShippingMethodId = settings.ScheduledShippingMethodId,
                ScheduledShippingMethodWithTruckId = settings.ScheduledShippingMethodWithTruckId,
                Price = settings.Price,
                ScheduledPrice = settings.ScheduledPrice,
                ScheduledPriceForHeavyOrBigBasket = settings.ScheduledPriceForHeavyOrBigBasket,
                PickupTruckPrice = settings.PickupTruckPrice,
                ApiToken = settings.ApiToken,
                ApiBaseUrl = settings.ApiBaseUrl,
                WeightMustCarryByPickupTruckInGrams = settings.WeightMustCarryByPickupTruckInGrams,
                VolumeMustCarryByPickupTruckInCM3 = settings.VolumeMustCarryByPickupTruckInCM3,
                OverSizeProductVolumeMustCarryByPickupTruckInCM3 = settings.OverSizeProductVolumeMustCarryByPickupTruckInCM3
            };

            foreach (var sm in await _shippingService.GetAllShippingMethodsAsync())
                model.AvailableShippingMethods.Add(new SelectListItem { Text = sm.Name, Value = sm.Id.ToString() });

            return View("~/Plugins/Delsami.Shipping.DelsamiLogistic/Areas/Admin/Views/DelsamiLogistic/Configure.cshtml", model);
        }


        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(DelsamiLogisticPermission.ManageDelsamiLogistic))
                return await AccessDeniedDataTablesJson();

            var settings = await _settingService.LoadSettingAsync<DelsamiLogisticSettings>();

            settings.ShippingMethodId = model.ShippingMethodId;
            settings.PickupTruckShippingMethodId = model.PickupTruckShippingMethodId;
            settings.ScheduledShippingMethodId = model.ScheduledShippingMethodId;
            settings.ScheduledShippingMethodWithTruckId = model.ScheduledShippingMethodWithTruckId;
            settings.Price = model.Price;
            settings.PickupTruckPrice = model.PickupTruckPrice;
            settings.ScheduledPrice = model.ScheduledPrice;
            settings.ScheduledPriceForHeavyOrBigBasket = model.ScheduledPriceForHeavyOrBigBasket;
            settings.ApiToken = model.ApiToken;
            settings.ApiBaseUrl = model.ApiBaseUrl;
            settings.WeightMustCarryByPickupTruckInGrams = model.WeightMustCarryByPickupTruckInGrams;
            settings.VolumeMustCarryByPickupTruckInCM3 = model.VolumeMustCarryByPickupTruckInCM3;
            settings.OverSizeProductVolumeMustCarryByPickupTruckInCM3 = model.OverSizeProductVolumeMustCarryByPickupTruckInCM3;

            await _settingService.SaveSettingAsync(settings);
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

            return RedirectToAction("Configure");
        }
        #endregion
    }
}
