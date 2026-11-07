using FluentMigrator.Builders.Create.Table;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Data.Mapping.Builders
{
    public class AvailableDeliveryDateTimeRangeBuilder : NopEntityBuilder<AvailableDeliveryDateTimeRange>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(AvailableDeliveryDateTimeRange.ExceptionDateId)).AsInt32().Nullable().ForeignKey<ExceptionDate>()
                .WithColumn(nameof(AvailableDeliveryDateTimeRange.AvailableDeliveryTimeRangeId)).AsInt32().ForeignKey<AvailableDeliveryTimeRange>()
                .WithColumn(nameof(AvailableDeliveryDateTimeRange.DayOfWeek)).AsInt32().Nullable()
                .WithColumn(nameof(AvailableDeliveryDateTimeRange.StartDateOnUtc)).AsDateTime2().Nullable()
                .WithColumn(nameof(AvailableDeliveryDateTimeRange.EndDateOnUtc)).AsDateTime2().Nullable();

        }
    }
}
