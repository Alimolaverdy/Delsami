using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Themes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NopShop.Plugin.Misc.LoginWithMobile.Components
{
    public class WidgetsChangePhonenumberDataViewComponent : NopViewComponent
    {
        private readonly IThemeContext _themeContext;

        public WidgetsChangePhonenumberDataViewComponent(IThemeContext themeContext)
        {
            _themeContext = themeContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var currentTheme =await _themeContext.GetWorkingThemeNameAsync();

            return View("~/Plugins/NopShop.Misc.LoginWithMobile/Themes/" + currentTheme.ToString() + "/Views/Shared/Components/ChangePhonenumber/Default.cshtml", additionalData);
        }
    }
}
