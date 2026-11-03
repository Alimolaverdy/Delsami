using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace Delsami.Plugin.Misc.OnePageCheckout.Infrastructure
{
    public class NobStartup : INopStartup
    {
        public int Order => 1;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //services.AddScoped<IDelsamiOnePageCheckoutFactory, DelsamiOnePageCheckoutFactory>();
        }
    }
}
