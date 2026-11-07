using Delsami.Plugin.Misc.Core.Events;
using Delsami.Plugin.Shipping.DelsamiLogistic.Domain;
using Delsami.Plugin.Shipping.DelsamiLogistic.Services;
using Nop.Services.Events;
using Nop.Services.Shipping;
using System;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.DelsamiLogistic.EventHandlers
{
    public class OrderCanceledEventHandlers : IConsumer<OrderCanceledEvent>
    {
        #region Fields

        private readonly IQueuedCanceledShipmentForSendToLogisticService _queuedCanceledShipmentForSendToLogisticService;
        private readonly IShipmentService _shipmentService;

        #endregion

        #region Ctor

        public OrderCanceledEventHandlers(IQueuedCanceledShipmentForSendToLogisticService queuedCanceledShipmentForSendToLogisticService,
                                          IShipmentService shipmentService)
        {
            _queuedCanceledShipmentForSendToLogisticService = queuedCanceledShipmentForSendToLogisticService;
            _shipmentService = shipmentService;
        }

        #endregion

        #region Methods

        public async Task HandleEventAsync(OrderCanceledEvent eventMessage)
        {
            if (eventMessage is null || eventMessage.Order is null)
                return;

            var shipments = await _shipmentService.GetShipmentsByOrderIdAsync(eventMessage.Order.Id);

            foreach (var shipment in shipments)
            {

                var queuedCanceledShipmentForSendToLogistic = new QueuedCanceledShipmentForSendToLogistic
                {
                    ShipmentId = shipment.Id,
                    CreatedDateTime = DateTime.UtcNow,
                    SentTries = 0,
                };

                await _queuedCanceledShipmentForSendToLogisticService.InsertAsync(queuedCanceledShipmentForSendToLogistic);
            }

        }

        #endregion
    }
}
