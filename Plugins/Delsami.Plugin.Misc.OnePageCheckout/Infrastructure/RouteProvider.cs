using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Delsami.Plugin.Misc.OnePageCheckout.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => 999;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(name: "DelsamiOnePageCheckout",
                pattern: $"onepagecheckout",
                defaults: new { controller = "DelsamiCheckout", action = "OnePageCheckout" });

            endpointRouteBuilder.MapControllerRoute(name: "DelsamiOnePageCheckoutAddAddress",
                pattern: $"/customer/addressadd",
                defaults: new { controller = "OnePageCheckout", action = "AddressAdd" });

            endpointRouteBuilder.MapControllerRoute(
                name: " DelsamiOnePageCheckoutCheckUserVpn",
                pattern: "Plugins/OnePageCheckout/CheckUserVpn",
                defaults: new { controller = "DelsamiCheckout", action = "CheckUserVpn" }
            );

            endpointRouteBuilder.MapControllerRoute(
                name: " checkOutComplete",
                pattern: "checkout/completed",
                defaults: new { controller = "DelsamiCheckout", action = "Completed" }
            );
        }
    }
}
