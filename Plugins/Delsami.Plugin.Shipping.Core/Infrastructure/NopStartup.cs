using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Delsami.Plugin.Shipping.Core.Areas.Admin.Factories;
using Delsami.Plugin.Shipping.Core.Services;
using Nop.Core.Infrastructure;

namespace Delsami.Plugin.Shipping.Core.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ILogisticsScheduleService, LogisticsScheduleService>();
            services.AddScoped<ILogisticsScheduleModelFactory, LogisticsScheduleModelFactory>();
            services.AddScoped<ILogisticsScheduleExceptionModelFactory, LogisticsScheduleExceptionModelFactory>();
            services.AddScoped<ILogisticsScheduleExceptionService, LogisticsScheduleExceptionService>();
            services.AddScoped<ICartonService, CartonService>();
            services.AddScoped<ICartonModelFactory, CartonModelFactory>();
        }

        public void Configure(IApplicationBuilder application)
        {
        }

        public int Order => 99999;
    }
}