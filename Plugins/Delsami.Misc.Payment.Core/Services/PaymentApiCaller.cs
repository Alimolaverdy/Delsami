using Delsami.Plugin.Misc.Core.Models.ApiCaller;
using Delsami.Plugin.Misc.Core.Models.Resilience;
using Delsami.Plugin.Misc.Core.Services.ApiCaller;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payment.Core.Services
{
    internal class PaymentApiCaller : IPaymentApiCaller
    {
        private readonly IApiCaller _apiCaller;

        public PaymentApiCaller(IApiCaller apiCaller)
        {
            _apiCaller = apiCaller;
        }

        public async Task<ApiResponse<TResponse>> GetAsync<TResponse>(
               string url,
               string bearerToken = null,
               Dictionary<string, string> headers = null,
               string contentType = null)
        {
            var resilienceOptions = new ResilienceOptions
            {
                EnableCircuitBreaker = false,
                EnableRetry = true,
                RetryCount = 3,
                RetryBaseDelaySeconds = 2,
                EnableTimeout = true,
                TimeoutSeconds = 10
            };

            return await _apiCaller.GetAsync<TResponse>(url, bearerToken, headers, null, contentType, resilienceOptions);
        }

        public async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(
                 string url,
               TRequest body,
               string bearerToken = null,
               Dictionary<string, string> headers = null,
               string contentType = null)

        {
            var resilienceOptions = new ResilienceOptions
            {
                EnableCircuitBreaker = false,
                EnableRetry = true,
                RetryCount = 3,
                RetryBaseDelaySeconds = 2,
                EnableTimeout = true,
            };

            return await _apiCaller.PostAsync<TRequest, TResponse>(url, body, bearerToken, headers, null, contentType, resilienceOptions);

        }
    }
}
