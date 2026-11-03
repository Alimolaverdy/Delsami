using Delsami.Plugin.Widgets.AddressLocation.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace Delsami.Plugin.Widgets.AddressLocation.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(StateProvinceLocation), $"D_AL_{nameof(StateProvinceLocation)}"},
            { typeof(AddressCity), $"D_AL_{nameof(AddressCity)}"},
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}
