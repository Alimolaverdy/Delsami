using Nop.Core;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Delsami.Plugin.Shipping.Core.Domain
{
    public class Carton : BaseEntity
    {
        public string Name { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public decimal Length { get; set; }

        public int PostBoxNumber { get; set; }
    }
}
