using Delsami.Plugin.Misc.Core.Models.Resilience;
using Delsami.Plugin.Misc.Core.Services.Resilience;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payment.Core.Services
{

    public class PaymentResilienceService : IPaymentResilienceService
    {
        private readonly IResilienceService _resilienceService;

        public PaymentResilienceService(IResilienceService resilienceService)
        {
            _resilienceService = resilienceService;
        }

        public async Task ExecuteAsync(string operationName,
                                       Func<CancellationToken, Task> action,
                                       CancellationToken cancellationToken = default)
        {
            var resilienceOptions = new ResilienceOptions
            {
                EnableCircuitBreaker = false,
                EnableRetry = true,
                RetryCount = 3,
                RetryBaseDelaySeconds = 2,
                EnableTimeout = true,
                TimeoutSeconds = 35
            };

            await _resilienceService.ExecuteAsync(operationName, action, resilienceOptions, cancellationToken);
        }

        public async Task<T> ExecuteAsync<T>(
            string operationName,
            Func<CancellationToken, Task<T>> action,
            CancellationToken cancellationToken = default)
        {
            var resilienceOptions = new ResilienceOptions
            {
                EnableCircuitBreaker = false,
                EnableRetry = true,
                RetryCount = 3,
                RetryBaseDelaySeconds = 2,
                EnableTimeout = true,
                TimeoutSeconds = 35
            };

            return await _resilienceService.ExecuteAsync<T>(operationName, action, resilienceOptions, cancellationToken);
        }
    }
}
