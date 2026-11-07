using Delsami.Plugin.Shipping.Core.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace Delsami.Plugin.Shipping.Core.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(Carton), $"D_SC_{nameof(Carton)}"},
            { typeof(LogisticsSchedule), $"D_SC_{nameof(LogisticsSchedule)}"},
            { typeof(LogisticsScheduleException), $"D_SC_{nameof(LogisticsScheduleException)}"}
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}
