using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Shipping.Core.Domain;
using Nop.Data.Mapping.Builders;

namespace Delsami.Plugin.Shipping.Core.Data.Mapping.Builder
{
    public class LogisticsScheduleBuilder : NopEntityBuilder<LogisticsSchedule>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(LogisticsSchedule.DayOfWeek)).AsInt16().NotNullable()
                .WithColumn(nameof(LogisticsSchedule.StartTime)).AsTime().NotNullable()
                .WithColumn(nameof(LogisticsSchedule.EndTime)).AsTime().NotNullable();
                //.WithColumn(nameof(LogisticsSchedule.ValidFrom)).AsDateTime2().Nullable()
                //.WithColumn(nameof(LogisticsSchedule.ValidTo)).AsDateTime2().Nullable();
        }
    }
}
