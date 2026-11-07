using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Common;
using Nop.Web.Controllers;
using Nop.Web.Framework.UI;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Infrastructure;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Controllers
{
    public class DeliverySchedulingController : BasePublicController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IShippingMethodCapacityService _shippingMethodCapacityService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IOrderShippingMethodCapacityMappingService _orderShippingMethodCapacityMappingService;

        public DeliverySchedulingController(
            IWorkContext workContext,
            IStoreContext storeContext,
            IShippingMethodCapacityService shippingMethodCapacityService,
            IGenericAttributeService genericAttributeService,
            IOrderShippingMethodCapacityMappingService orderShippingMethodCapacityMappingService,
            INopHtmlHelper nopHtmlHelper
            )
        {
            _workContext = workContext;
            _storeContext = storeContext;
            _shippingMethodCapacityService = shippingMethodCapacityService;
            _genericAttributeService = genericAttributeService;
            _orderShippingMethodCapacityMappingService = orderShippingMethodCapacityMappingService;
        }

        public virtual ActionResult LoadAvailableDeliveryDateTimesTable(string selectedShippingOption, bool isOpc)
        {
            return ViewComponent("LoadAvailableDeliveryDateTimesTable", new { selectedShippingOption, isOpc });
        }

        public virtual async Task<JsonResult> CheckAvailabilityForSelectedDateTime(int shippingMethodCapacityId, DateTime deliveryDateOnUtc, int availableDeliveryTimeRangeId, string deliveryTime, string selectedShippingOption)
        {
            var shippingMethodCapacity = await _shippingMethodCapacityService.GetShippingMethodCapacityByIdAsync(shippingMethodCapacityId);
            if (shippingMethodCapacity == null)
                throw new NullReferenceException(nameof(shippingMethodCapacity));

            var placedOrdersCounts = await _orderShippingMethodCapacityMappingService.GetPlacedOrdersCountAsync(storeId: (await _storeContext.GetCurrentStoreAsync()).Id);

            var placedOrdersCountForThisDateTime = 0;
            if (placedOrdersCounts.Any(t => t.ShippingMethodId == shippingMethodCapacity.ShippingMethodId && t.DeliveryDateOnUtc == deliveryDateOnUtc.Date && t.AvailableDeliveryTimeRangeId == availableDeliveryTimeRangeId))
                placedOrdersCountForThisDateTime = placedOrdersCounts.Single(t => t.ShippingMethodId == shippingMethodCapacity.ShippingMethodId && t.DeliveryDateOnUtc == deliveryDateOnUtc.Date && t.AvailableDeliveryTimeRangeId == availableDeliveryTimeRangeId).OrdersCount;

            var isAvailable = (shippingMethodCapacity.Capacity - placedOrdersCountForThisDateTime) > 0;

            if (isAvailable)
            {
                var splittedOption = selectedShippingOption.Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
                if (splittedOption.Length != 2)
                    throw new Exception("Selected shipping method can't be parsed");

                await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(), Constants.SystemCustomerAttributeNames_ShippingMethodCapacityId, shippingMethodCapacityId);
                await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(), Constants.SystemCustomerAttributeNames_DeliveryDateOnUtc, deliveryDateOnUtc);
                await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(), Constants.SystemCustomerAttributeNames_DeliveryTime, deliveryTime);

                return Json(new { Success = true });
            }

            return Json(new { Success = false });
        }

        public virtual async Task<JsonResult> RemoveSelectedDateTime()
        {
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(), Constants.SystemCustomerAttributeNames_ShippingMethodCapacityId, "");
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(), Constants.SystemCustomerAttributeNames_DeliveryDateOnUtc, "");
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(), Constants.SystemCustomerAttributeNames_DeliveryTime, "");

            return Json(new { Success = true });
        }
    }
}
