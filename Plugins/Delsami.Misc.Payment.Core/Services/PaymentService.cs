using Delsami.Plugin.Payment.Core.Enums;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Messages;
using NUglify.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using PaymentDomain = Delsami.Plugin.Payment.Core.Domain;

namespace Delsami.Plugin.Payment.Core.Services
{
    public class PaymentService : IPaymentService
    {
        #region Fields
        private readonly ILocker _locker;
        private readonly IRepository<PaymentDomain.Payment> _paymentRepo;
        private readonly INotificationService _notificationService;

        #endregion

        #region Ctor


        public PaymentService(IRepository<PaymentDomain.Payment> paymentRepo, INotificationService notificationService, ILocker locker)
        {
            _locker = locker;
            _paymentRepo = paymentRepo;
            _notificationService = notificationService;
        }

        #endregion

        #region Methods

        public async Task InsertAsync(PaymentDomain.Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            await _paymentRepo.InsertAsync(payment);
        }

        public async Task UpdateAsync(PaymentDomain.Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            await _paymentRepo.UpdateAsync(payment);
        }

        public async Task<Payment.Core.Domain.Payment> GetAsync(int id)
        {
            return await _paymentRepo.GetByIdAsync(id);
        }

        public virtual async Task<IPagedList<Payment.Core.Domain.Payment>> GetPagedListAsync(int? orderId = null, PaymentStatus? status = null, bool? completed = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var productCommissions = await _paymentRepo.GetAllPagedAsync(query =>
            {
                if (orderId.HasValue)
                    query = query.Where(w => w.OrderId == orderId.Value);

                if (status.HasValue)
                    query = query.Where(w => w.Status == status.Value);

                if (completed.HasValue)
                    query = query.Where(w => w.Completed == completed.Value);

                return query.OrderByDescending(c => c.Id);

            }, pageIndex, pageSize);

            return productCommissions;
        }

        public async Task<bool> IsValidForProccessing(int orderId)
        {
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var payments = await GetPagedListAsync(orderId);
            foreach (var payment in payments)
            {
                if (payment.Status != PaymentStatus.Failed)
                {
                    _notificationService.ErrorNotification(await localizationService.GetResourceAsync("Delsami.plugin.payment.core.raceconditionError"));
                    return false;
                }

            };

            return true;
        }

        public async Task PerformActionWithLockAsync(string resource, Func<Task> action, TimeSpan? expirationTime=null)
        {
            expirationTime ??= TimeSpan.FromSeconds(30);
            await _locker.PerformActionWithLockAsync(resource, expirationTime.Value, action);
        }
        #endregion
    }
}
