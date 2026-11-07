using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Services.Events;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Events
{
    public class ShippingMethodInsertEventConsumer : IConsumer<EntityInsertedEvent<ShippingMethod>>
    {
        private readonly IShippingMethodCapacityService _shippingMethodCapacityService;
        private readonly IAvailableDeliveryDateTimeRangeService _availableDeliveryDateTimeRangeService;

        public ShippingMethodInsertEventConsumer(IShippingMethodCapacityService shippingMethodCapacityService,
            IAvailableDeliveryDateTimeRangeService availableDeliveryDateTimeRangeService)
        {
            _shippingMethodCapacityService = shippingMethodCapacityService;
            _availableDeliveryDateTimeRangeService = availableDeliveryDateTimeRangeService;

        }

        public async Task HandleEventAsync(EntityInsertedEvent<ShippingMethod> eventMessage)
        {
            var availableDeliveryDateTimeRanges = (await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangesAsync()).Where(w => !w.EndDateOnUtc.HasValue);
            foreach (var availableDeliveryDateTimeRange in availableDeliveryDateTimeRanges)
            {
                await _shippingMethodCapacityService.InsertShippingMethodCapacityAsync(new Domain.ShippingMethodCapacity
                {
                    Capacity = 0,
                    AvailableDeliveryDateTimeRangeId = availableDeliveryDateTimeRange.Id,
                    ShippingMethodId = eventMessage.Entity.Id
                });
            }
        }
    }
}