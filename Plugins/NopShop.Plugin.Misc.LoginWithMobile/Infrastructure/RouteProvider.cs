using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace NopShop.Plugin.Misc.LoginWithMobile.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : BaseRouteProvider, IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var loginWithMobileSetting = EngineContext.Current.Resolve<LoginWithMobileSetting>();
            if (loginWithMobileSetting.Enable)
            {
                //get language pattern
                //it's not needed to use language pattern in AJAX requests and for actions returning the result directly (e.g. file to download),
                //use it only for URLs of pages that the user can go to
                var lang = GetLanguageRoutePattern();

                endpointRouteBuilder.MapControllerRoute("Plugin.Misc.LoginWithMobile.Login",
                    pattern: $"{lang}/Login/",
                    new { controller = "Customer", action = "LoginWithMobile" });

                endpointRouteBuilder.MapControllerRoute("Plugin.Misc.LoginWithMobile.LoginWithEmail", 
                    pattern: $"{lang}/LoginWithEmail/",
                    new { controller = "Customer", action = "LoginWithEmail" });

                if (!loginWithMobileSetting.EnableLoginWithEmail)
                {
                    endpointRouteBuilder.MapControllerRoute("Plugin.Misc.LoginWithMobile.LoginWithEmailRegister",
                        pattern: $"{lang}/register/",
                        new { controller = "Customer", action = "LoginWithEmailRegister" });

                    endpointRouteBuilder.MapControllerRoute("Plugin.Misc.LoginWithMobile.passwordrecovery",
                        pattern: $"{lang}/passwordrecovery/",
                        new { controller = "Customer", action = "LoginWithEmailpasswordrecovery" });
                }

                endpointRouteBuilder.MapControllerRoute("Plugin.Misc.LoginWithMobile.LoginCheckoutAsGuest", 
                    pattern: $"{lang}/login/checkoutasguest/",
                    new { controller = "Customer", action = "LoginWithMobile" });

                endpointRouteBuilder.MapControllerRoute("Plugin.Misc.LoginWithMobile.ChangePhonenumber", 
                    pattern: $"{lang}/ChangePhonenumber/",
                    new { controller = "Customer", action = "ChangePhonenumber" });

                endpointRouteBuilder.MapControllerRoute("Plugin.Misc.LoginWithMobile.ChangePhonenumberActivationCode", 
                    pattern: $"{lang}/ChangePhonenumberActivationCode/",
                    new { controller = "Customer", action = "ChangePhonenumberActivationCode" });

                endpointRouteBuilder.MapControllerRoute("Plugin.Misc.LoginWithMobile.GetActivationCode", 
                    pattern: $"{lang}/GetActivationCode/",
                    new { controller = "Customer", action = "GetActivationCode" });

                endpointRouteBuilder.MapControllerRoute("Plugin.Misc.LoginWithMobile.GetActivationCodeChange",
                    pattern: $"{lang}/GetActivationCodeChange/",
                    new { controller = "Customer", action = "GetActivationCodeChange" });

            }
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 100000;
    }
}