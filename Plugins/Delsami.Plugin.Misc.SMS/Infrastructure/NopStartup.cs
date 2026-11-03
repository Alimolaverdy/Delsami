using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Delsami.Plugin.Misc.SMS.Infrastructure.ExternalServices;
using Delsami.Plugin.Misc.SMS.Servicecs;

namespace Delsami.Plugin.Misc.SMS.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public int Order => 10000;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISMSSender, SMSSender>();
            services.AddScoped<IKavehNegarSMSSenderService, KavehNegarSMSSenderService>();
        }
    }
}
