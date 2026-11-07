using Microsoft.AspNetCore.Mvc;
using Delsami.Plugin.Payment.Core.Enums;
using Delsami.Plugin.Payment.Core.Models;
using Nop.Core.Domain.Orders;
using Nop.Services.Localization;
using Nop.Services.Orders;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payment.Core.Controllers;

public class NobPartsErrorPageController : Controller
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly IOrderService _orderService;

    #endregion

    #region Ctor

    public NobPartsErrorPageController(ILocalizationService localizationService, IOrderService orderService)
    {
        _localizationService = localizationService;
        _orderService = orderService;
    }

    #endregion

    #region Methods

    public async Task<IActionResult> PaymentError(int? orderId = null)
    {
        var model = new CheckoutFailedModel();

        //switch (errorCode)
        //{
        //    case ErrorCode.OrderNotFound:
        //        {
        //            model.ErrorMessage = await _localizationService.GetResourceAsync("NobParts.Plugin.Payment.Core.ErrorCode.OrderNotFound");
        //            break;
        //        }
        //    case ErrorCode.PaymentNotFound:
        //        {
        //            model.ErrorMessage = await _localizationService.GetResourceAsync("NobParts.Plugin.Payment.Core.ErrorCode.PaymentNotFound");
        //            break;
        //        }
        //    case ErrorCode.GatewayError:
        //        {
        //            model.ErrorMessage = await _localizationService.GetResourceAsync("NobParts.Plugin.Payment.Core.ErrorCode.GatewayError");
        //            model.OrderId = orderId;
        //            break;
        //        }
        //    case ErrorCode.GetTokenNotResponse:
        //        {
        //            model.ErrorMessage = await _localizationService.GetResourceAsync("NobParts.Plugin.Payment.Core.ErrorCode.GetTokenNotResponse");
        //            model.OrderId = orderId;
        //            break;
        //        }
        //    case ErrorCode.AmountValueNotEqual:
        //        {
        //            model.ErrorMessage = await _localizationService.GetResourceAsync("NobParts.Plugin.Payment.Core.ErrorCode.AmountValueNotEqual");
        //            model.OrderId = orderId;
        //            break;
        //        }
        //    case ErrorCode.StatusProblem:
        //        {
        //            model.ErrorMessage = await _localizationService.GetResourceAsync("NobParts.Plugin.Payment.Core.ErrorCode.StatusProblem");
        //            model.OrderId = orderId;
        //            break;
        //        }

        //}

        if (orderId.HasValue)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId.Value);

            if (order == null || (order.OrderStatus != OrderStatus.Pending && order.OrderStatus != OrderStatus.Cancelled))
                return RedirectToAction("Index", "Home");
        }
        model.OrderId = orderId;

        return View("~/Plugins/NobParts.Payment.Core/Views/NobPartsErrorPage/PaymentError.cshtml", model);
    }

    #endregion
}
