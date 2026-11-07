using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.UI;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Extensions;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Models;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Settings;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Components
{
    public class LoadOrderDetailsDeliveryDateTime : NopViewComponent
    {
        private readonly DeliverySchedulingSetting _deliverySchedulingSetting;
        private readonly IOrderShippingMethodCapacityMappingService _orderShippingMethodCapacityMappingService;
        private readonly ILocalizationService _localizationService;
        private readonly IShippingMethodCapacityService _shippingMethodCapacityService;
        private readonly IAvailableDeliveryDateTimeRangeService _availableDeliveryDateTimeRangeService;
        private readonly IAvailableDeliveryTimeRangeService _availableDeliveryTimeRangeService;

        public LoadOrderDetailsDeliveryDateTime(DeliverySchedulingSetting deliverySchedulingSetting,
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
            if (!_deliverySchedulingSetting.EnablePlugin || additionalData == null)
                return Content(string.Empty);

            var propertyName = "Id";
            var value = (int)additionalData.GetType().GetProperty(propertyName)?.GetValue(additionalData);

            var orderShippingMethodCapacityMapping = await _orderShippingMethodCapacityMappingService.GetOrderShippingMethodCapacityMappingByOrderIdAsync(value);

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

            var model = new OrderDetailsDeliveryDateTimeModel()
            {
                DeliveryDate = await orderShippingMethodCapacityMapping.DeliveryDateOnUtc.FormatDateTimeAsync(),
                DeliveryTime = await _localizationService.GetLocalizedAsync(availableDeliveryTimeRange, d => d.Time)
            };

            return View("~/Plugins/Delsami.OrderFulfillment.DeliveryScheduling/Views/Shared/Components/LoadOrderDetailsDeliveryDateTime/Default.cshtml", model);
        }
    }
}
