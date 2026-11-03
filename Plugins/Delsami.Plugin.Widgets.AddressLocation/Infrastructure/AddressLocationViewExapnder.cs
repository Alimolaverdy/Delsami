using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Delsami.Plugin.Widgets.AddressLocation.Infrastructure
{
    public class AddressLocationViewExapnder : IViewLocationExpander
    {
        private const string THEME_KEY = "nop.themename";

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.AreaName == "Admin" && context.ViewName.Equals("EditorTemplates/Address", StringComparison.InvariantCultureIgnoreCase))
            {
                viewLocations = new string[] { $"~/Plugins/Delsami.Widgets.AddressLocation/Areas/Admin/Views/Shared/EditorTemplates/Address.cshtml" }.Concat(viewLocations);
            }

            if (context.Values.TryGetValue(THEME_KEY, out string theme2) && context.AreaName == null && context.ViewName.Equals("_CreateOrUpdateAddress", StringComparison.InvariantCultureIgnoreCase))
            {
                viewLocations = new string[] { $"~/Plugins/Delsami.Widgets.AddressLocation/Views/Shared/_CreateOrUpdateAddress.cshtml" }.Concat(viewLocations);
            }

            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            //do nothing here
        }
    }
}
