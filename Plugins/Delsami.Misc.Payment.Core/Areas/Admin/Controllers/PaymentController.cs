using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using System.Threading.Tasks;
using Nop.Services.Security;
using Delsami.Plugin.Payment.Core.Infrastructure.Permissions;
using Delsami.Plugin.Payment.Core.Areas.Admin.Models;
using Delsami.Plugin.Payment.Core.Areas.Admin.Factories;
using Delsami.Plugin.Payment.Core.Services;
using System;

namespace Delsami.Plugin.Payment.Core.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class PaymentController : BasePluginController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IPaymentModelFactory _paymentModelFactory;
        private readonly IPaymentService _paymentService;

        #endregion

        #region Ctor

        public PaymentController(IPermissionService permissionService,
                                 IPaymentModelFactory paymentModelFactory,
                                 IPaymentService paymentService)
        {
            _permissionService = permissionService;
            _paymentModelFactory = paymentModelFactory;
            _paymentService = paymentService;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(PaymentCorePermission.ManagePayment))
                return AccessDeniedView();

            //prepare model
            var model = _paymentModelFactory.PreparePaymentSearchModel(new PaymentSearchModel());

            return View("~/Plugins/Delsami.Payment.Core/Areas/Admin/Views/Payment/List.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(PaymentSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(PaymentCorePermission.ManagePayment))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _paymentModelFactory.PreparePaymentListModelAsync(searchModel);

            return Json(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!await _permissionService.AuthorizeAsync(PaymentCorePermission.ManagePayment))
                return await AccessDeniedDataTablesJson();

            var payment = await _paymentService.GetAsync(id);

            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            var model = await _paymentModelFactory.PreparePaymentModelAsync(null, payment);

            return View("~/Plugins/Delsami.Payment.Core/Areas/Admin/Views/Payment/Details.cshtml", model);
        }

        #endregion
    }
}
