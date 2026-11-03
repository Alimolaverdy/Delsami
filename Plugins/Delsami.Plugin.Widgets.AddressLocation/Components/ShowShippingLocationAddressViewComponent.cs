using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using System.Threading.Tasks;
using Nop.Web.Framework.Components;

namespace Delsami.Plugin.Widgets.AddressLocation.Components
{
    public class ShowShippingLocationAddressViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;

        public ShowShippingLocationAddressViewComponent(IStoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            return View("~/Plugins/Delsami.Widgets.AddressLocation/Views/ShowShippingLocationAddress.cshtml", additionalData);
        }

    }
}
