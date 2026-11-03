using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Delsami.Plugin.Misc.SMS.Areas.Admin.Models
{
    public record ConfigurationModel : BaseNopModel
    {

        [NopResourceDisplayName("Delsami.Plugin.Misc.SMS.Configuration.ApiKey")]
        public string Apikey { get; set; }


        [NopResourceDisplayName("Delsami.Plugin.Misc.SMS.Configuration.SenderNumber")]
        public string SenderNumber { get; set; }


        [NopResourceDisplayName("Delsami.Plugin.Misc.SMS.Configuration.MaximumLengthOfSMS")]
        public int MaximumLengthOfSMS { get; set; }


        [NopResourceDisplayName("Delsami.Plugin.Misc.SMS.Configuration.BaseUrl")]
        public string BaseUrl { get; set; }
    }
}
