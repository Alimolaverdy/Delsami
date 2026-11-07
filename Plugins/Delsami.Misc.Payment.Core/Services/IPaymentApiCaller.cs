using Delsami.Plugin.Misc.Core.Models.ApiCaller;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payment.Core.Services
{
    public interface IPaymentApiCaller
    {
        Task<ApiResponse<TResponse>> GetAsync<TResponse>(
               string url,
               string bearerToken = null,
               Dictionary<string, string> headers = null,
               string contentType = null);

        Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(
               string url,
               TRequest body,
               string bearerToken = null,
               Dictionary<string, string> headers = null,
               string contentType = null);

    }
}
