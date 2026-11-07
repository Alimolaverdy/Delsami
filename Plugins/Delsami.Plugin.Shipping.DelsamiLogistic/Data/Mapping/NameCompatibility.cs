using Delsami.Plugin.Shipping.DelsamiLogistic.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace Delsami.Plugin.Shipping.DelsamiLogistic.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(QueuedCanceledShipmentForSendToLogistic), $"D_DL_{nameof(QueuedCanceledShipmentForSendToLogistic)}"}
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}
