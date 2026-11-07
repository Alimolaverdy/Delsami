using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;

namespace Delsami.Plugin.Payment.Core.Models;

public class VerifyingResult
{

    public IActionResult ActionResult { get; set; }

    public Order Order { get; set; }

    public Domain.Payment Payment { get; set; }

    public BasePaymentResponseModel VerifyResult { get; set; }

    public BasePaymentResponseModel ConfirmResult { get; set; }

    public PayResultModel PayResult { get; set; }

    public bool IsSuccess { get; set; }

    public bool IsConfirm { get; set; }

    public bool CanceledByUser { get; set; }
}
