using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using System.Threading.Tasks;
using Nop.Web.Framework.Components;
using Nop.Services.Configuration;

namespace Delsami.Plugin.Widgets.AddressLocation.Components
{
    public class ShowBillingLocationAddressViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;

        public ShowBillingLocationAddressViewComponent(IStoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            return View("~/Plugins/Delsami.Widgets.AddressLocation/Views/ShowBillingLocationAddress.cshtml", additionalData);
        }

    }
}
