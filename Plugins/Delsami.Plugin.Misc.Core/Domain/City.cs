using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Domain
{
    public class City : BaseEntity
    {
        public int StateProvinceId { get; set; }
        public string Name { get; set; }
        public bool Featured { get; set; }
    }
}
