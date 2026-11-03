using Delsami.Plugin.Misc.Core.Services;
using DocumentFormat.OpenXml.EMMA;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using NopShop.Plugin.Misc.LoginWithMobile.Helpers;
using NopShop.Plugin.Misc.LoginWithMobile.Models;
using NopShop.Plugin.Misc.LoginWithMobile.Validators;

namespace NopShop.Plugin.Misc.LoginWithMobile.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring plugin DB context on application startup
    /// </summary>
    /// <summary>
    /// Represents object for the configuring plugin DB context on application startup
    /// </summary>
    public class PluginDbStartup : INopStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<CustomViewLocationExpander>();

             //services.AddScoped<IFaraPayamakSmsHelper, FaraPayamakSmsHelper>();

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new CustomViewLocationExpander());
            });

            //services.AddValidatorsFromAssemblyContaining<LoginMobileValidator>(); // Replace with your validator class
            services.AddScoped<IValidator<LoginMobileModel>, LoginMobileValidator>();
            services.AddScoped<IOtpService, OtpService>();
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {

        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => int.MaxValue;
    }
}