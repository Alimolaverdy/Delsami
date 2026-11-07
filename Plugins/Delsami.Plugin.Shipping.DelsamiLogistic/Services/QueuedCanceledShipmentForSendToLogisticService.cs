using Delsami.Plugin.Shipping.DelsamiLogistic.Domain;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.DelsamiLogistic.Services
{
    public class QueuedCanceledShipmentForSendToLogisticService : IQueuedCanceledShipmentForSendToLogisticService
    {
        #region Fields

        private readonly IRepository<QueuedCanceledShipmentForSendToLogistic> _queuedCanceledShipmentForSendToLogisticRepo;

        #endregion

        #region Ctor

        public QueuedCanceledShipmentForSendToLogisticService(IRepository<QueuedCanceledShipmentForSendToLogistic> queuedCanceledShipmentForSendToLogisticRepo)
        {
            _queuedCanceledShipmentForSendToLogisticRepo = queuedCanceledShipmentForSendToLogisticRepo;
        }

        #endregion

        #region Methods

        public async Task InsertAsync(QueuedCanceledShipmentForSendToLogistic queuedCanceledShipmentForSendToLogistic)
        {
            if (queuedCanceledShipmentForSendToLogistic is null)
                throw new ArgumentNullException(nameof(queuedCanceledShipmentForSendToLogistic));

            await _queuedCanceledShipmentForSendToLogisticRepo.InsertAsync(queuedCanceledShipmentForSendToLogistic);
        }

        public async Task<IList<QueuedCanceledShipmentForSendToLogistic>> GetNotSentQueuedCanceledShipmentForSendToLogisticsAsync()
        {
            var notSentQueuedCanceledShipmentForSendToLogistics = await _queuedCanceledShipmentForSendToLogisticRepo.Table.Where(w => !w.Sent && w.SentTries <= 5).ToListAsync();

            return notSentQueuedCanceledShipmentForSendToLogistics;
        }

        public async Task UpdateAsync(QueuedCanceledShipmentForSendToLogistic queuedCanceledShipmentForSendToLogistic)
        {
            if (queuedCanceledShipmentForSendToLogistic is null)
                throw new ArgumentNullException(nameof(queuedCanceledShipmentForSendToLogistic));

            await _queuedCanceledShipmentForSendToLogisticRepo.UpdateAsync(queuedCanceledShipmentForSendToLogistic);
        }
        #endregion
    }
}
