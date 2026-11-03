using Microsoft.Extensions.DependencyInjection;
using Nop.Services.Logging;
using Polly.Wrap;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Models.Resilience;

namespace Delsami.Plugin.Misc.Core.Services.Resilience
{
    public class ResilienceService : IResilienceService
    {
        private readonly ResilienceOptions _defaultOptions;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ConcurrentDictionary<string, AsyncPolicyWrap> _policyCache = new();

        public ResilienceService(ResilienceOptions defaultOptions, IServiceScopeFactory scopeFactory)
        {
            _defaultOptions = defaultOptions;
            _scopeFactory = scopeFactory;
        }

        public async Task ExecuteAsync(
            string operationName,
            Func<CancellationToken, Task> action,
            ResilienceOptions options = null,
            CancellationToken cancellationToken = default)
        {
            var logger = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ILogger>();

            var policy = GetPolicy(operationName, options);

            try
            {
                await policy.ExecuteAsync(action, cancellationToken);
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync($"Final failure for operation {operationName}:{ex.Message}", ex);

                throw;
            }
        }

        public async Task<T> ExecuteAsync<T>(
            string operationName,
            Func<CancellationToken, Task<T>> action,
            ResilienceOptions options = null,
            CancellationToken cancellationToken = default)
        {
            var logger = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ILogger>();

            var policy = GetPolicy(operationName, options);

            try
            {
                return await policy.ExecuteAsync(action, cancellationToken);
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync($"Final failure for operation {operationName}: ${ex.Message}", ex);
                throw;
            }
        }

        private AsyncPolicyWrap GetPolicy(string operationName, ResilienceOptions overrideOptions)
        {
            var logger = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ILogger>();

            var opt = overrideOptions ?? _defaultOptions;

            var key = operationName;

            return _policyCache.GetOrAdd(key, _ =>
                PollyPolicyFactory.Build(opt, logger, operationName));
        }
    }
}
