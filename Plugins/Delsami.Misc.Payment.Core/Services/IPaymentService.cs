using Delsami.Plugin.Payment.Core.Enums;
using Nop.Core;
using System;
using System.Threading.Tasks;
using PaymentDomain = Delsami.Plugin.Payment.Core.Domain;

namespace Delsami.Plugin.Payment.Core.Services;

public interface IPaymentService
{
    Task InsertAsync(PaymentDomain.Payment payment);

    Task UpdateAsync(PaymentDomain.Payment payment);

    Task<Payment.Core.Domain.Payment> GetAsync(int id);

    Task<IPagedList<Payment.Core.Domain.Payment>> GetPagedListAsync(int? orderId = null, PaymentStatus? status = null, bool? completed = null, int pageIndex = 0, int pageSize = int.MaxValue);

    Task<bool> IsValidForProccessing(int orderId);

    Task PerformActionWithLockAsync(string resource, Func<Task> action, TimeSpan? expirationTime=null);
}
