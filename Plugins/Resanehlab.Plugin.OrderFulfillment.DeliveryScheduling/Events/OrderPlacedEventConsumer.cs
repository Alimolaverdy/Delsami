using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Common;
using Nop.Services.Events;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Infrastructure;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Settings;
using System;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Events
{
    public class OrderPlacedEventConsumer : IConsumer<OrderPlacedEvent>
    {
        private readonly DeliverySchedulingSetting _deliverySchedulingSetting;
        private readonly IWorkContext _workContext;
        private readonly IOrderShippingMethodCapacityMappingService _orderShippingMethodCapacityMappingService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAvailableDeliveryDateTimeRangeService _availableDeliveryDateTimeRangeService;
        private readonly IShippingMethodCapacityService _shippingMethodCapacityService;
        private readonly IAvailableDeliveryTimeRangeService _availableDeliveryTimeRangeService;

        public OrderPlacedEventConsumer(
            DeliverySchedulingSetting deliverySchedulingSetting,
            IWorkContext workContext,
            IOrderShippingMethodCapacityMappingService orderShippingMethodCapacityMappingService,
            IGenericAttributeService genericAttributeService,
            IAvailableDeliveryDateTimeRangeService availableDeliveryDateTimeRangeService,
            IShippingMethodCapacityService shippingMethodCapacityService,
            IAvailableDeliveryTimeRangeService availableDeliveryRangeService)
        {
            _deliverySchedulingSetting = deliverySchedulingSetting;
            _workContext = workContext;
            _orderShippingMethodCapacityMappingService = orderShippingMethodCapacityMappingService;
            _genericAttributeService = genericAttributeService;
            _availableDeliveryDateTimeRangeService = availableDeliveryDateTimeRangeService;
            _shippingMethodCapacityService = shippingMethodCapacityService;
            _availableDeliveryTimeRangeService = availableDeliveryRangeService;
        }

        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            if (!_deliverySchedulingSetting.EnablePlugin)
                return;


            if (eventMessage.Order is null)
                return;

            if (eventMessage.Order.ShippingStatus == ShippingStatus.ShippingNotRequired)
                return;

            if (eventMessage.Order.ShippingRateComputationMethodSystemName != "Delsami.Shipping.DelsamiLogistic")
                return;

            var shippingMethodCapacityId = await _genericAttributeService.GetAttributeAsync<int>(await _workContext.GetCurrentCustomerAsync(), Constants.SystemCustomerAttributeNames_ShippingMethodCapacityId);

            if (shippingMethodCapacityId == 0)
                return;

            var shippingMethodCapacity = await _shippingMethodCapacityService.GetShippingMethodCapacityByIdAsync(shippingMethodCapacityId);

            if (shippingMethodCapacity is null)
                return;

            var availableDeliveryDateTimeRange = await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangeByIdAsync(shippingMethodCapacity.AvailableDeliveryDateTimeRangeId);

            if (availableDeliveryDateTimeRange is null)
                return;

            var availableDeliveryTimeRange = await _availableDeliveryTimeRangeService.GetAvailableDeliveryTimeRangeByIdAsync(availableDeliveryDateTimeRange.AvailableDeliveryTimeRangeId);

            if (availableDeliveryTimeRange is null)
                return;

            var orderShippingMethodCapacityMapping = new OrderShippingMethodCapacityMapping()
            {
                OrderId = eventMessage.Order.Id,
                ShippingMethodCapacityId = await _genericAttributeService.GetAttributeAsync<int>(await _workContext.GetCurrentCustomerAsync(), Constants.SystemCustomerAttributeNames_ShippingMethodCapacityId),
                DeliveryDateOnUtc = await _genericAttributeService.GetAttributeAsync<DateTime>(await _workContext.GetCurrentCustomerAsync(), Constants.SystemCustomerAttributeNames_DeliveryDateOnUtc),
                Time = availableDeliveryTimeRange.Time,
                EndHour=availableDeliveryTimeRange.EndHour,
                StartHour=availableDeliveryTimeRange.StartHour,
                ShippingMethodId= shippingMethodCapacity.ShippingMethodId,

            };

            await _orderShippingMethodCapacityMappingService.InsertOrderShippingMethodCapacityMappingAsync(orderShippingMethodCapacityMapping);
        }
    }
}