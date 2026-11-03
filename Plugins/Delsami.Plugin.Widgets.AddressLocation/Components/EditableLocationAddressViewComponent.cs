using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;

namespace NobParts.Plugin.Widgets.AddressLocation.Components
{
    public class EditableLocationAddressViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly AddressLocationSettings _addressLocationSettings;

        public EditableLocationAddressViewComponent(IStoreContext storeContext,
                                            ISettingService settingService,
                                            AddressLocationSettings addressLocationSettings)
        {
            _storeContext = storeContext;
            _settingService = settingService;
            _addressLocationSettings = addressLocationSettings;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var currentUrl = Request.Path.Value?.ToLower();
            var isOnePageCheckout = currentUrl.Contains("/onepagecheckout") || Request.Headers["x-requested-with"] == "XMLHttpRequest";

            if (isOnePageCheckout)
                return View("~/Plugins/NobParts.Widgets.AddressLocation/Views/NopOnePageCheckoutEditableLocationAddress.cshtml", _addressLocationSettings);
            else
                return View("~/Plugins/NobParts.Widgets.AddressLocation/Views/EditableLocationAddress.cshtml", _addressLocationSettings);
        }

    }
}
