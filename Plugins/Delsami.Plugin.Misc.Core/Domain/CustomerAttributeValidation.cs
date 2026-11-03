using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Domain
{
    public class CustomerAttributeValidation : BaseEntity
    {
        public int CustomerAttributeId { get; set; }

        public string RegexPattern { get; set; }

        public bool IsNationalCode { get; set; }

        public bool PreventFromShoppingInInvalidState { get; set; }

        public bool Active { get; set; }


    }
}
