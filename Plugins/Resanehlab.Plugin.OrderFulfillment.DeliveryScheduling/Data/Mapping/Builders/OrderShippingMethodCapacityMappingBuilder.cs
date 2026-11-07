using FluentMigrator.Builders.Create.Table;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Data.Mapping.Builders
{
    public class OrderShippingMethodCapacityMappingBuilder : NopEntityBuilder<OrderShippingMethodCapacityMapping>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OrderShippingMethodCapacityMapping.ShippingMethodCapacityId)).AsInt32().Nullable().ForeignKey<ShippingMethodCapacity>()
                .WithColumn(nameof(OrderShippingMethodCapacityMapping.OrderId)).AsInt32().NotNullable()
                .WithColumn(nameof(OrderShippingMethodCapacityMapping.DeliveryDateOnUtc)).AsDateTime2().NotNullable()
                .WithColumn(nameof(OrderShippingMethodCapacityMapping.Time)).AsString().Nullable()
                .WithColumn(nameof(OrderShippingMethodCapacityMapping.EndHour)).AsTime().NotNullable()
                .WithColumn(nameof(OrderShippingMethodCapacityMapping.StartHour)).AsTime().NotNullable()
                .WithColumn(nameof(OrderShippingMethodCapacityMapping.ShippingMethodId)).AsInt32().NotNullable();
        }
    }
}
