using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Delsami.Plugin.Misc.Core;
using Delsami.Plugin.Misc.Core.Events;
using Delsami.Plugin.Misc.Core.Helpers;
using Delsami.Plugin.Payment.Core.Enums;
using Delsami.Plugin.Payment.Core.Models;
using Delsami.Plugin.Payment.Core.Services;
using Nop.Core.Caching;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Web.Framework.Controllers;
using System;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payment.Core.Controllers;

public abstract class BaseNopPartsPaymentController<T> : BasePaymentController where T : BasePaymentResponseModel
{
    #region Fields

    protected readonly IPaymentService _paymentService;
    protected readonly ILogger _logger;
    protected readonly IOrderService _orderService;
    protected readonly IOrderProcessingService _orderProcessingService;
    protected readonly INotificationService _notificationService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IEventPublisher _eventPublisher;
    private readonly CoreSettings _coreSettings;
    private readonly ILocker _paymentLocker;

    private VerifyingResult _verifyingResult;
    protected abstract PaymentGateway paymentGateway { get; }

    private static readonly TimeSpan VerifyLockWaitLimit = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan VerifyLockLease = TimeSpan.FromMinutes(3);
    private static readonly TimeSpan VerifyLockPoll = TimeSpan.FromMilliseconds(250);

    #endregion

    #region Ctor

    protected BaseNopPartsPaymentController(
        IPaymentService paymentService,
        IOrderService orderService,
        IOrderProcessingService orderProcessingService,
        ILogger logger,
        INotificationService notificationService,
        ILocalizationService localizationService,
        IEventPublisher eventPublisher,
        CoreSettings coreSettings,
        ILocker paymentLocker)
    {
        _eventPublisher = eventPublisher;
        _paymentService = paymentService;
        _orderService = orderService;
        _orderProcessingService = orderProcessingService;
        _logger = logger;
        _notificationService = notificationService;
        _localizationService = localizationService;
        _coreSettings = coreSettings;
        _paymentLocker = paymentLocker ?? throw new ArgumentNullException(nameof(paymentLocker));
    }

    #endregion

    #region BaseMethods

    public async Task<IActionResult> VerifyGet(int paymentId, T model)
    {
        // Preserves original route behavior; ideally do not allow state-changing GETs.
        return await Verify(paymentId, model);
    }

    [HttpPost]
    public async Task<IActionResult> Verify(int paymentId, T model)
    {
        if (paymentId <= 0)
            return RedirectToRoute("Checkout/PaymentError", new { ErrorCode.PaymentNotFound });

        // A single order can have several legitimate payment attempts.
        // Read the attempt only to establish the shared ORDER lock key.
        // The authoritative validation is repeated INSIDE the lock.
        Domain.Payment paymentForLock;
        try
        {
            HttpContext.RequestAborted.ThrowIfCancellationRequested();
            paymentForLock = await _paymentService.GetAsync(paymentId);
        }
        catch (OperationCanceledException) when (HttpContext.RequestAborted.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync(
                $"Payment lookup failed before acquiring verification lock: paymentId={paymentId}", ex);
            Response.Headers["Retry-After"] = "2";
            return StatusCode(503, "Payment verification temporarily unavailable.");
        }

        if (paymentForLock == null)
            return RedirectToRoute("Checkout/PaymentError", new { ErrorCode.PaymentNotFound });

        var orderId = paymentForLock.OrderId;
        if (orderId <= 0)
            return RedirectToRoute("Checkout/PaymentError", new { ErrorCode.PaymentNotFound });

        // IMPORTANT: no gateway or paymentId here. Every payment attempt for the
        // same order must serialize on this resource across ALL application nodes.
        var resource = $"Payment.Order.Verify.{orderId}";
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        while (stopwatch.Elapsed < VerifyLockWaitLimit)
        {
            HttpContext.RequestAborted.ThrowIfCancellationRequested();
            IActionResult actionResult = null;
            bool acquired;
            try
            {
                acquired = await _paymentLocker.PerformActionWithLockAsync(
                    resource,
                    VerifyLockLease,
                    async () =>
                    {
                        // Re-read payment/order after acquiring the shared order lock.
                        // Do not use the pre-lock payment snapshot for business effects.
                        actionResult = await VerifyCoreAsync(paymentId, orderId, model);
                    });
            }
            catch (Exception ex) when (ex is OperationCanceledException &&
                                       HttpContext.RequestAborted.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                // The action MAY already have executed (for example, lease loss,
                // Redis timeout, DB failure or gateway/monitoring failure).
                // Never automatically re-run business actions in this catch.
                await _logger.ErrorAsync(
                    $"Payment verification uncertain: gateway={paymentGateway}, " +
                    $"orderId={orderId}, paymentId={paymentId}", ex);
                Response.Headers["Retry-After"] = "2";
                return StatusCode(503, "Payment verification state is uncertain; reconciliation may be required.");
            }

            if (acquired)
            {
                if (actionResult != null)
                    return actionResult;
                await _logger.ErrorAsync(
                    $"Locked payment verification returned no action: orderId={orderId}, paymentId={paymentId}");
                Response.Headers["Retry-After"] = "2";
                return StatusCode(503, "Payment verification state is uncertain.");
            }

            // Non-blocking, cancellable wait; do not sleep beyond the local budget.
            var remaining = VerifyLockWaitLimit - stopwatch.Elapsed;
            if (remaining <= TimeSpan.Zero)
                break;
            await Task.Delay(remaining < VerifyLockPoll ? remaining : VerifyLockPoll,
                HttpContext.RequestAborted);
        }

        Response.Headers["Retry-After"] = "2";
        return StatusCode(503, "Another verification for this order is already in progress.");
    }

    private async Task<IActionResult> VerifyCoreAsync(int paymentId, int expectedOrderId, T model)
    {
        // Keep exceptions visible to Verify: an exception may occur AFTER gateway
        // confirmation, and treating it as an ordinary redirect is unsafe.
        _verifyingResult = await PaymentValidation(paymentId);

        if (!_verifyingResult.IsSuccess)
            return _verifyingResult.ActionResult;

        // A changed mapping must not let us operate on another order while
        // holding the old order's lock.
        if (_verifyingResult.Payment.OrderId != expectedOrderId ||
            _verifyingResult.Order.Id != expectedOrderId)
            throw new InvalidOperationException(
                "Payment-to-order association changed while acquiring the verification lock.");

        var verifyingResult = await VerifyPayment(
            _verifyingResult.Payment, _verifyingResult.Order, model);

        var actionResult = verifyingResult.IsSuccess
            ? await SuccessPath(verifyingResult)
            : await FailedPath(verifyingResult);


        return actionResult ?? Redirect("/");
    }

    protected virtual async Task<IActionResult> SuccessPath(VerifyingResult verifyResult)
    {
        var note = $"پرداخت با موفقیت انجام شد";

        if (verifyResult.PayResult != null)
            note += $" - اطلاعات پرداخت: {verifyResult.PayResult.ToString()}";

        if (verifyResult.VerifyResult != null)
            note += $" - اطلاعات تایید تراکنش: {verifyResult.VerifyResult.ToString()}";

        if (verifyResult.ConfirmResult != null)
            note += $" - اطلاعات سومین قدم تراکنش: {verifyResult.ConfirmResult.ToString()}";

        var orderNote = new OrderNote
        {
            Note = note,
            CreatedOnUtc = DateTime.UtcNow,
            DisplayToCustomer = false,
            OrderId = verifyResult.Order.Id
        };

        verifyResult.Order.CaptureTransactionId = verifyResult.PayResult.ReferenceNumber;

        await _orderService.UpdateOrderAsync(verifyResult.Order);

        verifyResult.Payment.ReferenceNumber = verifyResult.PayResult.ReferenceNumber;
        verifyResult.Payment.TrackId = verifyResult.PayResult.TrackId;
        verifyResult.Payment.ResultCode = verifyResult.PayResult.ResultCode;
        verifyResult.Payment.Status = verifyResult.IsConfirm ? PaymentStatus.Confirmed : PaymentStatus.Verified;

        if (verifyResult.IsConfirm)
            verifyResult.Payment.ConfirmDateTime = DateTime.UtcNow;
        else
            verifyResult.Payment.VerifyDateTime = DateTime.UtcNow;

        verifyResult.Payment.Completed = true;
        verifyResult.Payment.MaskedCardNumber = verifyResult.PayResult.MaskedCardNumber;
        verifyResult.Payment.HashedCardNumber = verifyResult.PayResult.HashedCardNumber;
        verifyResult.Payment.AdditionalData = verifyResult.PayResult.AdditionalData;

        await _paymentService.UpdateAsync(verifyResult.Payment);

        await _orderService.InsertOrderNoteAsync(orderNote);

        await _orderProcessingService.MarkOrderAsPaidAsync(verifyResult.Order);

        await _eventPublisher.PublishAsync(new OrderProcessingStartEvent(verifyResult.Order));

        return RedirectToAction("completed", "checkout", new { orderId = verifyResult.Order.Id });
    }

    protected virtual async Task<IActionResult> FailedPath(VerifyingResult verifyResult)
    {
        string note = string.Empty;

        if (verifyResult.PayResult != null)
            note = $"پرداخت از درگاه  {paymentGateway.ToString()} با خطا مواجه شد. اطلاعات پرداخت : {verifyResult.PayResult.ToString()}";

        if (verifyResult.VerifyResult != null)
            note = $"تایید پرداخت از درگاه {paymentGateway.ToString()} با خطا مواجه شد. اطلاعات تایید پرداخت: {verifyResult.VerifyResult.ToString()}";

        if (verifyResult.ConfirmResult != null)
            note = $"سومین قدم از درگاه {paymentGateway.ToString()} با خطا مواجه شد. اطلاعات سومین قدم: {verifyResult.ConfirmResult.ToString()}";

        if (!string.IsNullOrEmpty(note))
        {

            var orderNote = new OrderNote
            {
                Note = note,
                CreatedOnUtc = DateTime.UtcNow,
                DisplayToCustomer = false,
                OrderId = verifyResult.Order.Id
            };

            await _orderService.InsertOrderNoteAsync(orderNote);
        }

        verifyResult.Payment.Status = PaymentStatus.Failed;
        verifyResult.Payment.CanceledByUser = verifyResult.CanceledByUser;
        await _paymentService.UpdateAsync(verifyResult.Payment);

        if (verifyResult.ActionResult is null)
            return base.RedirectToRoute("RefillBasket");

        return verifyResult.ActionResult;
    }

    protected virtual async Task<VerifyingResult> PaymentValidation(int paymentId)
    {
        var payment = await _paymentService.GetAsync(paymentId);

        if (payment == null)
        {
            await _logger.ErrorAsync($"{paymentGateway} - payment not found. paymentId={paymentId}");
            return new()
            {
                ActionResult = RedirectToRoute("Checkout/PaymentError"),
                IsSuccess = false,
            };
        }

        var order = await _orderService.GetOrderByIdAsync(payment.OrderId);
        if (order == null)
        {
            await _logger.ErrorAsync($"{paymentGateway} - order not found. orderId={payment.OrderId}");
            return new()
            {
                ActionResult = RedirectToRoute("Checkout/PaymentError"),
                IsSuccess = false,
            };
        }

        // Different payment attempts may refer to the same order. Once the
        // order is paid, do not verify or finalize another payment attempt.
        if (order.PaymentStatus == Nop.Core.Domain.Payments.PaymentStatus.Paid ||
            order.OrderStatus == OrderStatus.Processing ||
            order.OrderStatus == OrderStatus.Complete)
            return new()
            {
                ActionResult = RedirectToAction("completed", "checkout", new { orderId = order.Id }),
                IsSuccess = false
            };

        // A previous callback may have saved the payment as completed but failed
        // while marking the order paid. Don't call the gateway a second time:
        // this requires reconciliation of the payment/order state.
        if (payment.Completed)
        {
            await _logger.ErrorAsync(
                $"Payment marked completed while order is not paid: " +
                $"gateway={paymentGateway}, paymentId={paymentId}, orderId={order.Id}");
            Response.Headers["Retry-After"] = "2";
            return new()
            {
                ActionResult = StatusCode(503, "Payment/order reconciliation required."),
                IsSuccess = false
            };
        }

        if (payment.CanceledByUser)
            return new()
            {
                IsSuccess = false,
                ActionResult = RedirectToRoute("RefillBasket")
            };

        if (!_orderProcessingService.CanMarkOrderAsPaid(order))
            return new()
            {
                ActionResult = RedirectToRoute("Checkout/PaymentError", new { order.Id }),
                IsSuccess = false,
            };

        return new() { IsSuccess = true, Order = order, Payment = payment };
    }

    protected VerifyingResult GenerateFailedResult(ErrorCode errorCode,
                                                   PayResultModel payModel = null,
                                                   BasePaymentResponseModel verifyModel = null,
                                                   BasePaymentResponseModel thirdStep = null)
    {

        return new VerifyingResult
        {
            IsSuccess = false,
            Payment = _verifyingResult.Payment,
            Order = _verifyingResult.Order,
            VerifyResult = verifyModel,
            ConfirmResult = thirdStep,
            IsConfirm = false,
            PayResult = payModel,
            CanceledByUser = errorCode == ErrorCode.CanceledByUser,
            ActionResult = errorCode == ErrorCode.CanceledByUser ?
            RedirectToRoute("RefillBasket") :
            RedirectToRoute("Checkout/PaymentError", new { orderId = _verifyingResult.Order.Id })
        };
    }

    protected VerifyingResult GenerateSuccessResult(PayResultModel payResult,
                                                    BasePaymentResponseModel verifyResult,
                                                    bool isConfirm,
                                                    BasePaymentResponseModel confirmResult = null)
    {
        return new()
        {
            IsSuccess = true,
            Payment = _verifyingResult.Payment,
            Order = _verifyingResult.Order,
            VerifyResult = verifyResult,
            ConfirmResult = confirmResult,
            IsConfirm = isConfirm,
            PayResult = payResult,
            ActionResult = null,

        };
    }
    #endregion

    #region Abstracts

    protected abstract Task<VerifyingResult> VerifyPayment(Domain.Payment payment, Order order, T model = default);

    #endregion

}

