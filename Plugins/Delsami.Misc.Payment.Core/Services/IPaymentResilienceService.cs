using System;
using System.Threading;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payment.Core.Services
{
    public interface IPaymentResilienceService
    {
        Task ExecuteAsync(string operationName,
                                       Func<CancellationToken, Task> action,
                                       CancellationToken cancellationToken = default);

        Task<T> ExecuteAsync<T>(
            string operationName,
            Func<CancellationToken, Task<T>> action,
            CancellationToken cancellationToken = default);
    }
}
