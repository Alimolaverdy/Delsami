using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.UI;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Extensions;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Infrastructure;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Settings;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Components
{
    public class OrderDeliveryDateTime : NopViewComponent
    {
        private readonly DeliverySchedulingSetting _deliverySchedulingSetting;
        private readonly IOrderShippingMethodCapacityMappingService _orderShippingMethodCapacityMappingService;
        private readonly ILocalizationService _localizationService;
        private readonly IShippingMethodCapacityService _shippingMethodCapacityService;
        private readonly IAvailableDeliveryDateTimeRangeService _availableDeliveryDateTimeRangeService;
        private readonly IAvailableDeliveryTimeRangeService _availableDeliveryTimeRangeService;

        public OrderDeliveryDateTime(DeliverySchedulingSetting deliverySchedulingSetting,
            IOrderShippingMethodCapacityMappingService orderShippingMethodCapacityMappingService,
            ILocalizationService localizationService,
            IShippingMethodCapacityService shippingMethodCapacityService,
            IAvailableDeliveryDateTimeRangeService availableDeliveryDateTimeRangeService,
            IAvailableDeliveryTimeRangeService availableDeliveryTimeRangeService,
            INopHtmlHelper nopHtmlHelper)
        {
            _deliverySchedulingSetting = deliverySchedulingSetting;
            _orderShippingMethodCapacityMappingService = orderShippingMethodCapacityMappingService;
            _localizationService = localizationService;
            _shippingMethodCapacityService = shippingMethodCapacityService;
            _availableDeliveryDateTimeRangeService = availableDeliveryDateTimeRangeService;
            _availableDeliveryTimeRangeService = availableDeliveryTimeRangeService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            if (!_deliverySchedulingSetting.EnablePlugin)
                return Content(string.Empty);

            if (additionalData is OrderModel)
            {
                var orderModel = (OrderModel)additionalData;

                var orderShippingMethodCapacityMapping = await _orderShippingMethodCapacityMappingService.GetOrderShippingMethodCapacityMappingByOrderIdAsync(orderModel.Id);
                if (orderShippingMethodCapacityMapping == null)
                    return Content(string.Empty);

                var shippingMethodCapacity = await _shippingMethodCapacityService.GetShippingMethodCapacityByIdAsync(orderShippingMethodCapacityMapping.ShippingMethodCapacityId);
                if (shippingMethodCapacity == null)
                    return Content(string.Empty);

                var availableDeliveryDateTimeRange = await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangeByIdAsync(shippingMethodCapacity.AvailableDeliveryDateTimeRangeId);
                if (availableDeliveryDateTimeRange == null)
                    return Content(string.Empty);

                var availableDeliveryTimeRange = await _availableDeliveryTimeRangeService.GetAvailableDeliveryTimeRangeByIdAsync(availableDeliveryDateTimeRange.AvailableDeliveryTimeRangeId);
                if (availableDeliveryTimeRange == null)
                    return Content(string.Empty);

                var model = new DeliverySchedulingOrderModel()
                {
                    DeliveryTimeRange = orderShippingMethodCapacityMapping != null ? await orderShippingMethodCapacityMapping.DeliveryDateOnUtc.FormatDateTimeAsync() : await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.NoData"),
                    DeliveryTime = orderShippingMethodCapacityMapping != null ? await _localizationService.GetLocalizedAsync(availableDeliveryTimeRange, d => d.Time) : await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.NoData"),
                    ShippingMethodName = orderShippingMethodCapacityMapping != null ? orderModel.ShippingMethod : await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.NoData")
                };

                return View("~/Plugins/Delsami.OrderFulfillment.DeliveryScheduling/Areas/Admin/Views/Shared/Components/OrderDeliveryDateTime/Default.cshtml", model);
            }

            return Content(string.Empty);
        }
    }
}