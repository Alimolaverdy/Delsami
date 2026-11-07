using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Data.Mapping.Builders
{
    public class ExceptionDateBuilder : NopEntityBuilder<ExceptionDate>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
               .WithColumn(nameof(ExceptionDate.Title)).AsString().Nullable()
               .WithColumn(nameof(ExceptionDate.DateOnUtc)).AsDateTime2().NotNullable()
               .WithColumn(nameof(ExceptionDate.StoreId)).AsInt32().NotNullable()
               .WithColumn(nameof(ExceptionDate.Deleted)).AsBoolean().NotNullable();
        }
    }
}
