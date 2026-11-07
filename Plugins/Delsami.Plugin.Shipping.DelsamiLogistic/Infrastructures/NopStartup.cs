using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Delsami.Plugin.Shipping.DelsamiLogistic.Services;
using Nop.Core.Infrastructure;

namespace Delsami.Plugin.Shipping.DelsamiLogistic.Infrastructures
{
    public class NopStartup : INopStartup
    {
        public int Order => int.MaxValue;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDelsamiLogisticService, DelsamiLogisticService>();
            services.AddScoped<IDelsamiLogisticApiService, DelsamiLogisticApiService>();
            services.AddScoped<IQueuedCanceledShipmentForSendToLogisticService, QueuedCanceledShipmentForSendToLogisticService>();
            
        }
    }
}
