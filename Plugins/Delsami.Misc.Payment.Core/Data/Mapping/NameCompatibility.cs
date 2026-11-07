using Nop.Data.Mapping;
using System;
using System.Collections.Generic;
using PaymentDomain = Delsami.Plugin.Payment.Core.Domain;

namespace Delsami.Plugin.Payment.Core.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(PaymentDomain.Payment), $"D_P_{nameof(PaymentDomain.Payment)}"},
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}
