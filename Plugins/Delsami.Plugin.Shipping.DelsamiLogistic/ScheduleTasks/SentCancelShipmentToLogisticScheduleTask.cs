using Delsami.Plugin.Shipping.DelsamiLogistic.Services;
using Nop.Core.Domain.Orders;
using Nop.Services.ScheduleTasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.DelsamiLogistic.ScheduleTasks
{
    public class SentCancelShipmentToLogisticScheduleTask : IScheduleTask
    {
        #region Fields

        private readonly IQueuedCanceledShipmentForSendToLogisticService _queuedCanceledShipmentForSendToLogisticService;
        private readonly IDelsamiLogisticApiService _DelsamiLogisticApiService;

        #endregion

        #region Ctor

        public SentCancelShipmentToLogisticScheduleTask(IQueuedCanceledShipmentForSendToLogisticService queuedCanceledShipmentForSendToLogisticService,
                                                        IDelsamiLogisticApiService DelsamiLogisticApiService)
        {
            _queuedCanceledShipmentForSendToLogisticService = queuedCanceledShipmentForSendToLogisticService;
            _DelsamiLogisticApiService = DelsamiLogisticApiService;
        }

        #endregion

        #region Methods

        public async Task ExecuteAsync()
        {
            var orderStatus = new List<int>() { (int)OrderStatus.Pending };


            var notSentQueuedCanceledShipmentForSendToLogistics = await _queuedCanceledShipmentForSendToLogisticService.GetNotSentQueuedCanceledShipmentForSendToLogisticsAsync();

            foreach (var queuedCanceledShipmentForSendToLogistic in notSentQueuedCanceledShipmentForSendToLogistics)
            {
                var result = await _DelsamiLogisticApiService.CloseCargoAndOrderBySellSiteAsync(queuedCanceledShipmentForSendToLogistic.ShipmentId);

                var now = DateTime.UtcNow;

                queuedCanceledShipmentForSendToLogistic.SentTries += 1;
                queuedCanceledShipmentForSendToLogistic.lastTryDateTime = now;
                queuedCanceledShipmentForSendToLogistic.Result = result.message;
                queuedCanceledShipmentForSendToLogistic.ResultStatusCode = result.statusCode;

                if (result.value || result.statusCode == 1002)
                {
                    queuedCanceledShipmentForSendToLogistic.Sent = true;
                    queuedCanceledShipmentForSendToLogistic.SentDateTime = now;
                }

                await _queuedCanceledShipmentForSendToLogisticService.UpdateAsync(queuedCanceledShipmentForSendToLogistic);
            }
        }

        #endregion
    }
}
