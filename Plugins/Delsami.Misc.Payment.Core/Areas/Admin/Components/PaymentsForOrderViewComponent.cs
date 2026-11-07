using Microsoft.AspNetCore.Mvc;
using Delsami.Plugin.Payment.Core.Areas.Admin.Models;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payment.Core.Areas.Admin.Components
{
    public class PaymentsForOrderViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public PaymentsForOrderViewComponent(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        #endregion

        public async Task<IViewComponentResult> InvokeAsync(OrderModel additionalData)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return Content("");

            var searchModel = new PaymentSearchModel();

            searchModel.OrderId = additionalData.Id;

            searchModel.SetGridPageSize();

            return View("~/Plugins/Delsami.Payment.Core/Areas/Admin/Views/Shared/Components/PaymentsForOrder/Default.cshtml", searchModel);
        }
    }
}
