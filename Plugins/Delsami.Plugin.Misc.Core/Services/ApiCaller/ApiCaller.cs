using Newtonsoft.Json;
using Delsami.Plugin.Misc.Core.Models.ApiCaller;
using Delsami.Plugin.Misc.Core.Models.Resilience;
using Delsami.Plugin.Misc.Core.Services.Resilience;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Services.ApiCaller;

public class ApiCaller : IApiCaller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;
    private readonly IResilienceService _resilienceService;

    public ApiCaller(
        IHttpClientFactory httpClientFactory,
        IResilienceService resilienceService,
        ILogger logger)
    {
        _httpClientFactory = httpClientFactory;
        _resilienceService = resilienceService;
        _logger = logger;
    }

    public async Task<ApiResponse<TResponse>> SendAsync<TResponse>(ApiRequest request)
    {
        var responseModel = new ApiResponse<TResponse>();

        try
        {
            var client = _httpClientFactory.CreateClient("CoreApiClient");

            client.Timeout = request.TimeoutSeconds.HasValue
                ? TimeSpan.FromSeconds(request.TimeoutSeconds.Value)
                : Timeout.InfiniteTimeSpan;

            HttpResponseMessage httpResponse = null;

            await _resilienceService.ExecuteAsync(request.Url, async ct =>
            {
                using var httpRequest = CreateHttpRequestMessage(request);

                httpResponse = await client.SendAsync(
                    httpRequest,
                    HttpCompletionOption.ResponseHeadersRead,
                    ct);
            }, request.ResilienceOptions);

            var raw = await httpResponse.Content.ReadAsStringAsync();

            responseModel.StatusCode = httpResponse.StatusCode;
            responseModel.RawResponse = raw;
            responseModel.IsSuccess = httpResponse.IsSuccessStatusCode;

            if (httpResponse.IsSuccessStatusCode)
            {
                if (!string.IsNullOrWhiteSpace(raw))
                {
                    responseModel.Data = JsonConvert.DeserializeObject<TResponse>(raw);
                }
            }
            else
            {
                responseModel.ErrorMessage = raw;
                await _logger.WarningAsync($"API Error: {(int)httpResponse.StatusCode} - {raw}");
            }
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("API Exception", ex);

            responseModel.IsSuccess = false;
            responseModel.ErrorMessage = ex.Message;
        }

        return responseModel;
    }

    public Task<ApiResponse<TResponse>> GetAsync<TResponse>(
        string url,
        string bearerToken = null,
        Dictionary<string, string> headers = null,
        int? timeoutSeconds = 15,
        string contentType = null,
        ResilienceOptions resilienceOptions = null

        )
    {
        return SendAsync<TResponse>(new ApiRequest
        {
            Url = url,
            Method = HttpMethod.Get,
            Headers = headers,
            BearerToken = bearerToken,
            TimeoutSeconds = timeoutSeconds,
            ContentType = contentType,
            ResilienceOptions = resilienceOptions
        });
    }

    public Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(
        string url,
        TRequest body,
        string bearerToken = null,
        Dictionary<string, string> headers = null,
        int? timeoutSeconds = 15,
        string contentType = null,
        ResilienceOptions resilienceOptions = null
        )
    {
        return SendAsync<TResponse>(new ApiRequest
        {
            Url = url,
            Method = HttpMethod.Post,
            Body = body,
            Headers = headers,
            BearerToken = bearerToken,
            TimeoutSeconds = timeoutSeconds,
            ContentType = contentType,
            ResilienceOptions = resilienceOptions

        });
    }

    private HttpRequestMessage CreateHttpRequestMessage(ApiRequest request)
    {
        var httpRequest = new HttpRequestMessage(request.Method, request.Url);

        if (request.Headers != null)
        {

            foreach (var header in request.Headers)
            {
                httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        if (!string.IsNullOrWhiteSpace(request.BearerToken))
        {
            httpRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", request.BearerToken);
        }

        if (request.Body != null)
        {
            if (request.ContentType == "application/x-www-form-urlencoded")
            {
                httpRequest.Content = new FormUrlEncodedContent(
                    (Dictionary<string, string>)request.Body);
            }
            else
            {
                var json = JsonConvert.SerializeObject(request.Body);
                httpRequest.Content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");
            }
        }

        return httpRequest;
    }
}
