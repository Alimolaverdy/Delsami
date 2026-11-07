using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Mvc;
using Delsami.Plugin.Shipping.Core.Areas.Admin.Factories;
using Delsami.Plugin.Shipping.Core.Areas.Admin.Models.LogisticsSchedules;
using Delsami.Plugin.Shipping.Core.Domain;
using Delsami.Plugin.Shipping.Core.Services;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.Core.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class LogisticsScheduleController : BasePluginController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly ILogisticsScheduleModelFactory _logisticsScheduleFactory;
        private readonly ILogisticsScheduleService _logisticsScheduleService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        #endregion

        #region Ctor

        public LogisticsScheduleController(IPermissionService permissionService,
                                          ILogisticsScheduleModelFactory logisticsScheduleFactory,
                                          ILogisticsScheduleService logisticsScheduleService,
                                          ILocalizationService localizationService,
                                          INotificationService notificationService)
        {
            _permissionService = permissionService;
            _logisticsScheduleFactory = logisticsScheduleFactory;
            _logisticsScheduleService = logisticsScheduleService;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //prepare model
            var model = _logisticsScheduleFactory.PrepareLogisticsScheduleSearchModel(new LogisticsScheduleSearchModel());

            return View("~/Plugins/Delsami.Shipping.Core/Areas/Admin/Views/LogisticsSchedules/List.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(LogisticsScheduleSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _logisticsScheduleFactory.PrepareLogisticsScheduleListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #region Create / Edit / Delete

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //prepare model
            var model = _logisticsScheduleFactory.PrepareLogisticsScheduleModel(new LogisticsScheduleModel(), null);

            return View("~/Plugins/Delsami.Shipping.Core/Areas/Admin/Views/LogisticsSchedules/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(LogisticsScheduleModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var isExist = await _logisticsScheduleService.IsExistDayOfWeek(model.DayOfWeek);

            if (isExist)
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShippingCore.Message.LogisticsSchedule.IsExistDayOfWeek"));

            if (ModelState.IsValid)
            {
                if (!await _logisticsScheduleService.IsExistDayOfWeek(model.DayOfWeek))
                {

                    var logisticsSchedule = model.ToEntity<LogisticsSchedule>();

                    await _logisticsScheduleService.InsertAsync(logisticsSchedule);

                    _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShippingCore.Message.LogisticsSchedule.Added"));
                }

                return RedirectToAction("List");
            }

            //prepare model
            model = _logisticsScheduleFactory.PrepareLogisticsScheduleModel(model, null);

            //if we got this far, something failed, redisplay form
            return View("~/Plugins/Delsami.Shipping.Core/Areas/Admin/Views/LogisticsSchedules/Create.cshtml", model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //try to get a LogisticsSchedule with the specified id
            var logisticsSchedule = await _logisticsScheduleService.GetLogisticsScheduleAsync(id);
            if (logisticsSchedule == null)
                return RedirectToAction("List");

            //prepare model
            var model = _logisticsScheduleFactory.PrepareLogisticsScheduleModel(null, logisticsSchedule);

            return View("~/Plugins/Delsami.Shipping.Core/Areas/Admin/Views/LogisticsSchedules/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(LogisticsScheduleModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //try to get a LogisticsSchedule with the specified id
            var LogisticsSchedule = await _logisticsScheduleService.GetLogisticsScheduleAsync(model.Id);

            if (LogisticsSchedule == null)
                return RedirectToAction("List");

            var dayOfWeek = model.DayOfWeek;

            var isExist = await _logisticsScheduleService.IsExistDayOfWeek(dayOfWeek, model.Id);

            if (isExist)
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShippingCore.Message.LogisticsSchedule.IsExistDayOfWeek"));

            if (ModelState.IsValid)
            {
                LogisticsSchedule = model.ToEntity(LogisticsSchedule);

                await _logisticsScheduleService.UpdateAsync(LogisticsSchedule);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShippingCore.Message.LogisticsSchedule.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = LogisticsSchedule.Id });
            }

            //prepare model
            model = _logisticsScheduleFactory.PrepareLogisticsScheduleModel(model, LogisticsSchedule);

            //if we got this far, something failed, redisplay form
            return View("~/Plugins/Delsami.Shipping.Core/Areas/Admin/Views/LogisticsSchedules/Edit.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //try to get a LogisticsSchedule with the specified id
            var LogisticsSchedule = await _logisticsScheduleService.GetLogisticsScheduleAsync(id);

            if (LogisticsSchedule == null)
                return RedirectToAction("List");

            await _logisticsScheduleService.DeleteAsync(LogisticsSchedule);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShippingCore.Message.LogisticsSchedule.Deleted"));

            return RedirectToAction("List");
        }


        #endregion
    }
}
