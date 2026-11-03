using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Delsami.Plugin.Misc.Core.Components
{
    public class ToEnglishNumberViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Delsami.Misc.Core/Views/Shared/Components/ToEnglishNumber/Default.cshtml");
        }
    }
}
