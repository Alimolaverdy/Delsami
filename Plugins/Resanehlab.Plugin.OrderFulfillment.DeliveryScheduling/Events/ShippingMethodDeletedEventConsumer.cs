using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Services.Events;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Settings;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Events
{
    public class ShippingMethodDeletedEventConsumer : IConsumer<EntityDeletedEvent<ShippingMethod>>
    {
        private readonly IShippingMethodCapacityService _shippingMethodCapacityService;

        public ShippingMethodDeletedEventConsumer(IShippingMethodCapacityService shippingMethodCapacityService)
        {
            _shippingMethodCapacityService = shippingMethodCapacityService;
        }

        public async Task HandleEventAsync(EntityDeletedEvent<ShippingMethod> eventMessage)
        {
            var shippingMethodCapacities = await _shippingMethodCapacityService.GetShippingMethodCapacitiesAsync(shippingMethodId: eventMessage.Entity.Id);
            foreach (var shippingMethodCapacity in shippingMethodCapacities)
                await _shippingMethodCapacityService.DeleteShippingMethodCapacityAsync(shippingMethodCapacity);
        }
    }
}
