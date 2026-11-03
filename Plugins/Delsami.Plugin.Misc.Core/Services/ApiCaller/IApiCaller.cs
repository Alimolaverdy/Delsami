using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Models.ApiCaller;
using Delsami.Plugin.Misc.Core.Models.Resilience;

namespace Delsami.Plugin.Misc.Core.Services.ApiCaller
{
    public interface IApiCaller
    {
        Task<ApiResponse<TResponse>> SendAsync<TResponse>(ApiRequest request);

        Task<ApiResponse<TResponse>> GetAsync<TResponse>(
            string url,
            string bearerToken = null,
            Dictionary<string, string> headers = null,
            int? timeoutSeconds = 15,
            string contentType = null,
            ResilienceOptions resilienceOptions = null);

        Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(
              string url,
            TRequest body,
            string bearerToken = null,
            Dictionary<string, string> headers = null,
            int? timeoutSeconds = 15,
            string contentType = null,
            ResilienceOptions resilienceOptions = null);
    }
}