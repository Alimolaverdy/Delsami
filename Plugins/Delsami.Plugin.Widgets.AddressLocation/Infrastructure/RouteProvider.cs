using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Delsami.Plugin.Widgets.AddressLocation.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => 999;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(name: "DelsamiAddressLocationAddAddress",
                pattern: $"/customer/addressadd",
                defaults: new { controller = "AddressLocation", action = "AddressAdd" }
            );

            endpointRouteBuilder.MapControllerRoute(name: "DelsamiAddressLocationEditAddress",
                pattern: $"/customer/addressedit/{{addressId:min(0)}}",
                defaults: new { controller = "AddressLocation", action = "AddressEdit" }
            );
        }
    }
}
