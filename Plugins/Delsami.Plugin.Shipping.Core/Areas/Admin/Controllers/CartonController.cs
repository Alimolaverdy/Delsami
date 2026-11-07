using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Delsami.Plugin.Shipping.Core.Areas.Admin.Models.Cartons;
using Delsami.Plugin.Shipping.Core.Services;
using Delsami.Plugin.Shipping.Core.Areas.Admin.Factories;
using Delsami.Plugin.Shipping.Core.Domain;

namespace Delsami.Plugin.Shipping.Core.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class CartonController : BasePluginController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly ICartonModelFactory _cartonFactory;
        private readonly ICartonService _cartonService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        #endregion

        #region Ctor

        public CartonController(IPermissionService permissionService,
                                ICartonModelFactory cartonFactory,
                                ICartonService cartonService,
                                ILocalizationService localizationService,
                                INotificationService notificationService)
        {
            _permissionService = permissionService;
            _cartonFactory = cartonFactory;
            _cartonService = cartonService;
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
            var model = _cartonFactory.PrepareCartonSearchModel(new CartonSearchModel());

            return View("~/Plugins/Delsami.Shipping.Core/Areas/Admin/Views/Carton/List.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(CartonSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _cartonFactory.PrepareCartonListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #region Create / Edit / Delete

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //prepare model
            var model = _cartonFactory.PrepareCartonModel(new CartonModel(), null);

            return View("~/Plugins/Delsami.Shipping.Core/Areas/Admin/Views/Carton/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(CartonModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var carBrandModel = model.ToEntity<Carton>();

                await _cartonService.InsertAsync(carBrandModel);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShippingCore.Message.Carton.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = carBrandModel.Id });
            }

            //prepare model
            model = _cartonFactory.PrepareCartonModel(model, null);

            //if we got this far, something failed, redisplay form
            return View("~/Plugins/Delsami.Shipping.Core/Areas/Admin/Views/Carton/Create.cshtml", model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //try to get a Carton with the specified id
            var carton = await _cartonService.GetCartonAsync(id);
            if (carton == null)
                return RedirectToAction("List");

            //prepare model
            var model = _cartonFactory.PrepareCartonModel(null, carton);

            return View("~/Plugins/Delsami.Shipping.Core/Areas/Admin/Views/Carton/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(CartonModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //try to get a Carton with the specified id
            var Carton = await _cartonService.GetCartonAsync(model.Id);

            if (Carton == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {

                Carton = model.ToEntity(Carton);

                await _cartonService.UpdateAsync(Carton);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShippingCore.Message.Carton.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = Carton.Id });
            }

            //prepare model
            model = _cartonFactory.PrepareCartonModel(model, Carton);

            //if we got this far, something failed, redisplay form
            return View("~/Plugins/Delsami.Shipping.Core/Areas/Admin/Views/Carton/Edit.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //try to get a Carton with the specified id
            var Carton = await _cartonService.GetCartonAsync(id);

            if (Carton == null)
                return RedirectToAction("List");

            await _cartonService.DeleteAsync(Carton);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShippingCore.Message.Carton.Deleted"));

            return RedirectToAction("List");
        }


        #endregion
    }
}
