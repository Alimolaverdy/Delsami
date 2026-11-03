using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Models.Resilience;

namespace Delsami.Plugin.Misc.Core.Services.Resilience
{
    public interface IResilienceService
    {
        Task ExecuteAsync(
            string operationName,
            Func<CancellationToken, Task> action,
            ResilienceOptions options = null,
            CancellationToken cancellationToken = default);

        Task<T> ExecuteAsync<T>(
            string operationName,
            Func<CancellationToken, Task<T>> action,
            ResilienceOptions options = null,
            CancellationToken cancellationToken = default);
    }
}
