using Delsami.Plugin.Shipping.DelsamiLogistic.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.DelsamiLogistic.Services
{
    public interface IQueuedCanceledShipmentForSendToLogisticService
    {
        Task InsertAsync(QueuedCanceledShipmentForSendToLogistic queuedCanceledShipmentForSendToLogistic);

        Task<IList<QueuedCanceledShipmentForSendToLogistic>> GetNotSentQueuedCanceledShipmentForSendToLogisticsAsync();

        Task UpdateAsync(QueuedCanceledShipmentForSendToLogistic queuedCanceledShipmentForSendToLogistic);

    }
}
