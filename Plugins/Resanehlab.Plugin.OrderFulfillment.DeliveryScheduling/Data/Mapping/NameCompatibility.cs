using Nop.Data.Mapping;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;
using System;
using System.Collections.Generic;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(AvailableDeliveryDateTimeRange), "D_DS_AvailableDeliveryDateTimeRange"},
            { typeof(AvailableDeliveryTimeRange), "D_DS_AvailableDeliveryTimeRange"},
            { typeof(ExceptionDate), "D_DS_ExceptionDate"},
            { typeof(OrderShippingMethodCapacityMapping), "D_DS_OrderShippingMethodCapacityMapping"},
            { typeof(OverriddenStartDisplayDaysOffset), "D_DS_OverriddenStartDisplayDaysOffset"},
            { typeof(ShippingMethodCapacity), "D_DS_ShippingMethodCapacity"}
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}