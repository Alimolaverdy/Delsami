using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;
using Nop.Data.Mapping.Builders;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Data.Mapping.Builders
{
    public class AvailableDeliveryTimeRangeBuilder : NopEntityBuilder<AvailableDeliveryTimeRange>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(AvailableDeliveryTimeRange.Time)).AsString().NotNullable()
                .WithColumn(nameof(AvailableDeliveryTimeRange.StoreId)).AsInt32().NotNullable()
                .WithColumn(nameof(AvailableDeliveryTimeRange.DisplayOrder)).AsInt32().NotNullable()
                .WithColumn(nameof(AvailableDeliveryTimeRange.Deleted)).AsBoolean().NotNullable()
                .WithColumn(nameof(AvailableDeliveryTimeRange.DaysOfWeek)).AsString().NotNullable()
                .WithColumn(nameof(AvailableDeliveryTimeRange.ShippingMethodIds)).AsString().NotNullable()
                .WithColumn(nameof(AvailableDeliveryTimeRange.StartHour)).AsCustom("Time(0)").NotNullable()
                .WithColumn(nameof(AvailableDeliveryTimeRange.EndHour)).AsCustom("Time(0)").NotNullable();
        }
    }
}
