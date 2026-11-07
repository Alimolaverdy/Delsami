using Nop.Web.Framework.Models;

namespace Delsami.Plugin.Payment.Core.Areas.Admin.Models
{
    public record PaymentSearchModel : BaseSearchModel
    {
        public int? OrderId { get; set; }
    }
}
