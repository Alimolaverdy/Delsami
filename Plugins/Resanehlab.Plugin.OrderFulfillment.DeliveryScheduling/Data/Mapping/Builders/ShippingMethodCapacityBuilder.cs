using FluentMigrator.Builders.Create.Table;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Data.Mapping.Builders
{
    public class ShippingMethodCapacityBuilder : NopEntityBuilder<ShippingMethodCapacity>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ShippingMethodCapacity.ShippingMethodId)).AsInt32().Nullable()
                .WithColumn(nameof(ShippingMethodCapacity.Capacity)).AsInt32().Nullable()
                .WithColumn(nameof(ShippingMethodCapacity.Deleted)).AsBoolean().Nullable()
                .WithColumn(nameof(ShippingMethodCapacity.AvailableDeliveryDateTimeRangeId)).AsInt32().Nullable().ForeignKey<AvailableDeliveryDateTimeRange>();
        }
    }
}
