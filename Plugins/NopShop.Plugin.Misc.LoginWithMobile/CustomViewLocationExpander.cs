using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;
using Nop.Core.Infrastructure;


namespace NopShop.Plugin.Misc.LoginWithMobile
{
    public class CustomViewLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            var loginWithMobileSetting = EngineContext.Current.Resolve<LoginWithMobileSetting>();

            if (loginWithMobileSetting.Enable)
            {
                if (context.Values.TryGetValue(THEME_KEY, out string theme))
                {
                    if (context.AreaName == null && context.ViewName == "Components/HeaderLinks/Default")
                    {
                        viewLocations = new string[] { $"/Plugins/NopShop.Misc.LoginWithMobile/Themes/{theme}/Views/Shared/Components/HeaderLinks/Default.cshtml"
                            }.Concat(viewLocations);
                    }
                    else if (context.AreaName == null && context.ViewName == "Components/AditionalNavLinks/Default")
                    {
                        viewLocations = new string[] { $"/Plugins/NopShop.Misc.LoginWithMobile/Themes/{theme}/Views/Shared/Components/AditionalNavLinks/Default.cshtml"
                            }.Concat(viewLocations);
                    }
                    else
                    {
                        viewLocations = new[] {
                        $"/Plugins/NopShop.Misc.LoginWithMobile/Themes/{theme}/Views/Shared/{{0}}.cshtml"
                        //$"/Plugins/NopShop.Misc.LoginWithMobile/Themes/{theme}/Views/Customer/{{0}}.cshtml",
                    }.Concat(viewLocations);
                    }
                }
            }

            return viewLocations;
        }

        private const string THEME_KEY = "nop.themename";

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            //no need to add the themeable view locations at all as the administration should not be themeable anyway
            //if (context.AreaName?.Equals(AreaNames.Admin) ?? false)
            //    return;

            //var themeContext = (IThemeContext)context.ActionContext.HttpContext.RequestServices.GetService(typeof(IThemeContext));
            //context.Values[THEME_KEY] = themeContext.WorkingThemeName;
        }
    }
}
