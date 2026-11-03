using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Delsami.Plugin.Misc.Core.Helpers;
using Delsami.Plugin.Misc.Core.Models.Resilience;
using Delsami.Plugin.Misc.Core.Services;
using Delsami.Plugin.Misc.Core.Services.ApiCaller;
using Delsami.Plugin.Misc.Core.Services.CityService;
using Delsami.Plugin.Misc.Core.Services.Resilience;
using Delsami.Plugin.Misc.Core.Services.ShippingMethod;
using Delsami.Plugin.Misc.Core.Services.ShippingMethodService;

namespace Delsami.Plugin.Misc.Core.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public int Order => 10000;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IApiCaller, ApiCaller>();
            services.AddScoped<ICityService, CityService>();
            services.AddSingleton(new ResilienceOptions());
            services.AddScoped<IResilienceService, ResilienceService>();
            services.AddScoped<IShippingMethodService, ShippingMethodService>();
            services.AddScoped<ICustomCustomerAttributesXMLService, CustomCustomerAttributesXMLService>();
            services.AddScoped<ICustomerAttributeValidationService, CustomerAttributeValidationService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IReservedStockQuantityHistoryService, ReservedStockQuantityHistoryService>();
            services.AddScoped<IDelsamiCustomerService, DelsamiCustomerService>();
            services.AddScoped<IEnumHelper, EnumHelper>();
            services.AddScoped<ICustomerProfileValidationService, CustomerProfileValidationService>();

        }
    }
}
