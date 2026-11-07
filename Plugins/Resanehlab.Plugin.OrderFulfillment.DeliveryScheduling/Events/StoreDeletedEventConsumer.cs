using Nop.Core.Domain.Stores;
using Nop.Core.Events;
using Nop.Services.Events;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Events
{
    public class StoreDeletedEventConsumer : IConsumer<EntityDeletedEvent<Store>>
    {
        private readonly IAvailableDeliveryTimeRangeService _availableDeliveryTimeRangeService;
        private readonly IExceptionDateService _exceptionDateService;
        public StoreDeletedEventConsumer(IAvailableDeliveryTimeRangeService availableDeliveryTimeRangeService,
            IExceptionDateService exceptionDateService)
        {
            _availableDeliveryTimeRangeService = availableDeliveryTimeRangeService;
            _exceptionDateService = exceptionDateService;
        }
        public async Task HandleEventAsync(EntityDeletedEvent<Store> eventMessage)
        {

            var availableDeliveryTimeRanges = await _availableDeliveryTimeRangeService.GetAvailableDeliveryTimeRangesAsync(eventMessage.Entity.Id);

            foreach (var availableDeliveryTimeRange in availableDeliveryTimeRanges)
                await _availableDeliveryTimeRangeService.DeleteAvailableDeliveryTimeRangeAsync(availableDeliveryTimeRange);

            var exceptionDates = await _exceptionDateService.GetExceptionDatesAsync(eventMessage.Entity.Id);

            foreach (var exceptionDate in exceptionDates)
                await _exceptionDateService.DeleteExceptionDateAsync(exceptionDate);
        }
    }
}
