using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Models.Resilience
{
    public class ResilienceOptions
    {
        // Retry
        public bool EnableRetry { get; set; } = true;
        public int RetryCount { get; set; } = 3;
        public int RetryBaseDelaySeconds { get; set; } = 2;

        // Circuit Breaker
        public bool EnableCircuitBreaker { get; set; } = true;
        public int ExceptionsAllowedBeforeBreaking { get; set; } = 1;
        public int BreakDurationSeconds { get; set; } = 30;

        // Timeout
        public bool EnableTimeout { get; set; } = true;
        public int TimeoutSeconds { get; set; } = 15;
    }
}