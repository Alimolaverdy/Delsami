using Nop.Web.Framework.Mvc.ModelBinding;

namespace Delsami.Plugin.Misc.Core.Areas.Admin.Models
{
    public class ConfigurationModel

    {

		[NopResourceDisplayName("Delsami.Plugin.Misc.Core.Configuration.NationalCodeAttributeId")]
        public int NationalCodeAttributeId { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.Core.Configuration.AllProductsCategoryId")]
        public int AllProductsCategoryId { get; set; }


    }
}
