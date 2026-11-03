using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Themes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NopShop.Plugin.Misc.LoginWithMobile.Components
{
    public class WidgetsAditionalMobileNavLinksDataViewComponent : NopViewComponent
    {
        private readonly IThemeContext _themeContext;

        public WidgetsAditionalMobileNavLinksDataViewComponent(IThemeContext themeContext)
        {
            _themeContext = themeContext;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var currentTheme = _themeContext.GetWorkingThemeNameAsync();

            return View("~/Plugins/NopShop.Misc.LoginWithMobile/Themes/" + currentTheme + "/Views/Shared/Components/AditionalNavLinks/Default.cshtml", additionalData);
        }
    }
}
