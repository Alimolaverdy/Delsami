using System;
using System.Collections.Generic;
using System.Linq;
using Polly.Wrap;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Models.Resilience;
using Nop.Services.Logging;
using Polly;
using Polly.Timeout;

namespace Delsami.Plugin.Misc.Core.Services.Resilience
{
    public static class PollyPolicyFactory
    {
        public static AsyncPolicyWrap Build(ResilienceOptions opt, ILogger logger, string operationName)
        {
            var policies = new List<IAsyncPolicy>();

            // Retry
            if (opt.EnableRetry)
            {
                policies.Add(
                    Policy.Handle<Exception>()
                        .WaitAndRetryAsync(
                            opt.RetryCount,
                            retryAttempt => TimeSpan.FromSeconds(Math.Pow(opt.RetryBaseDelaySeconds, retryAttempt)),
                            (exception, delay, retryCount, context) =>
                            {
                                logger.WarningAsync($"Retry {retryCount}/{opt.RetryCount} for {operationName}. Waiting {delay.TotalSeconds}s", exception);
                            }));
            }

            // Circuit Breaker
            if (opt.EnableCircuitBreaker)
            {
                policies.Add(
                    Policy.Handle<Exception>()
                        .CircuitBreakerAsync(
                            opt.ExceptionsAllowedBeforeBreaking,
                            TimeSpan.FromSeconds(opt.BreakDurationSeconds),
                            (exception, breakDelay) =>
                            {
                                logger.ErrorAsync($"Circuit opened for {operationName}. Duration {breakDelay.TotalSeconds}s", exception);
                            },
                            () => logger.InformationAsync($"Circuit reset for {operationName}"),
                            () => logger.InformationAsync($"Circuit half-open for {operationName}")
                        ));
            }

            // Timeout
            if (opt.EnableTimeout)
            {
                policies.Add(
                    Policy.TimeoutAsync(
                        TimeSpan.FromSeconds(opt.TimeoutSeconds),
                        TimeoutStrategy.Pessimistic,
                        (context, timespan, task) =>
                        {
                            logger.ErrorAsync($"Timeout after {timespan.TotalSeconds}s for {operationName}");
                            return Task.CompletedTask;
                        }));
            }

            if (!policies.Any())
                return Policy.WrapAsync(Policy.NoOpAsync());

            return Policy.WrapAsync(policies.ToArray());
        }
    }
}
