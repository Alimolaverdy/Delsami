using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Shipping.Core.Domain;
using Nop.Data.Mapping.Builders;

namespace Delsami.Plugin.Shipping.Core.Data.Mapping.Builder
{
    public class LogisticsScheduleExceptionBuilder : NopEntityBuilder<LogisticsScheduleException>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(LogisticsScheduleException.ExceptionDate)).AsDateTime2().NotNullable()
                .WithColumn(nameof(LogisticsScheduleException.StartTime)).AsTime().NotNullable()
                .WithColumn(nameof(LogisticsScheduleException.EndTime)).AsTime().NotNullable()
                .WithColumn(nameof(LogisticsScheduleException.IsHoliday)).AsBoolean().NotNullable();
        }
    }
}
