using Microsoft.AspNetCore.Mvc;
using Delsami.Plugin.Shipping.Core.Areas.Admin.Factories;
using Delsami.Plugin.Shipping.Core.Areas.Admin.Models.LogisticsSchedulesException;
using Delsami.Plugin.Shipping.Core.Domain;
using Delsami.Plugin.Shipping.Core.Services;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.Core.Areas.Admin.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.Admin)]
[AutoValidateAntiforgeryToken]
public class LogisticsScheduleExceptionController : BasePluginController
{
    #region Fields

    private readonly IPermissionService _permissionService;
    private readonly ILogisticsScheduleExceptionModelFactory _logisticsScheduleExceptionFactory;
    private readonly ILogisticsScheduleExceptionService _logisticsScheduleExceptionService;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;

    #endregion

    #region Ctor

    public LogisticsScheduleExceptionController(IPermissionService permissionService,
                                                ILogisticsScheduleExceptionModelFactory logisticsScheduleExceptionFactory,
                                                ILogisticsScheduleExceptionService logisticsScheduleExceptionService,
                                                ILocalizationService localizationService,
                                                INotificationService notificationService)
    {
        _permissionService = permissionService;
        _logisticsScheduleExceptionFactory = logisticsScheduleExceptionFactory;
        _logisticsScheduleExceptionService = logisticsScheduleExceptionService;
        _localizationService = localizationService;
        _notificationService = notificationService;
    }

    #endregion

    #region List

    [HttpPost]
    public virtual async Task<IActionResult> List(LogisticsScheduleExceptionSearchModel searchModel)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return await AccessDeniedDataTablesJson();

        //prepare model
        var model = await _logisticsScheduleExceptionFactory.PrepareLogisticsScheduleExceptionListModelAsync(searchModel);

        return Json(model);
    }

    #endregion

    #region Create / Edit / Delete

    public virtual async Task<IActionResult> Create()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();

        //prepare model
        var model = new LogisticsScheduleExceptionModel();

        return View("~/Plugins/Delsami.Shipping.Core/Areas/Admin/Views/LogisticsSchedulesException/Create.cshtml", model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    public virtual async Task<IActionResult> Create(LogisticsScheduleExceptionModel model, bool continueEditing)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();

        var exception = await _logisticsScheduleExceptionService.IsExistException(model.ExceptionDate);

        if (exception != null)
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShippingCore.Message.LogisticsScheduleException.ExistException"));

        if (ModelState.IsValid)
        {

            var logisticsScheduleException = model.ToEntity<LogisticsScheduleException>();

            await _logisticsScheduleExceptionService.InsertAsync(logisticsScheduleException);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShippingCore.Message.LogisticsScheduleException.Added"));

            return RedirectToAction("List", "LogisticsSchedule", new { area = "admin" });
        }


        return View("~/Plugins/Delsami.Shipping.Core/Areas/Admin/Views/LogisticsSchedulesException/Create.cshtml", model);
    }

    public virtual async Task<IActionResult> Edit(int id)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();

        var logisticsScheduleException = await _logisticsScheduleExceptionService.GetById(id);
        if (logisticsScheduleException == null)
            return RedirectToAction("List", "LogisticsSchedule", new { area = "admin" });

        var model = logisticsScheduleException.ToModel<LogisticsScheduleExceptionModel>();

        return View("~/Plugins/Delsami.Shipping.Core/Areas/Admin/Views/LogisticsSchedulesException/Edit.cshtml", model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    public virtual async Task<IActionResult> Edit(LogisticsScheduleExceptionModel model, bool continueEditing)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();

        //try to get a LogisticsSchedule with the specified id
        var LogisticsScheduleException = await _logisticsScheduleExceptionService.GetById(model.Id);

        if (LogisticsScheduleException == null)
            return RedirectToAction("List", "LogisticsSchedule", new { area = "admin" });

        var exception = await _logisticsScheduleExceptionService.IsExistException(model.ExceptionDate);

        if (exception != null && exception.Id!=model.Id)
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShippingCore.Message.LogisticsSchedule.ExistException"));

        if (ModelState.IsValid)
        {
            LogisticsScheduleException = model.ToEntity(LogisticsScheduleException);

            await _logisticsScheduleExceptionService.UpdateAsync(LogisticsScheduleException);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShippingCore.Message.LogisticsScheduleException.Updated"));

            if (!continueEditing)
                return RedirectToAction("List", "LogisticsSchedule", new { area = "admin" });

            return RedirectToAction("Edit", new { id = LogisticsScheduleException.Id });
        }

        return View("~/Plugins/Delsami.Shipping.Core/Areas/Admin/Views/LogisticsSchedulesException/Edit.cshtml", model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Delete(int id)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();

        var LogisticsScheduleException = await _logisticsScheduleExceptionService.GetById(id);

        if (LogisticsScheduleException == null)
            return RedirectToAction("List", "LogisticsSchedule", new { area = "admin" });

        await _logisticsScheduleExceptionService.DeleteAsync(LogisticsScheduleException);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShippingCore.Message.LogisticsScheduleException.Deleted"));

        return RedirectToAction("List", "LogisticsSchedule", new { area = "admin" });
    }


    #endregion
}
