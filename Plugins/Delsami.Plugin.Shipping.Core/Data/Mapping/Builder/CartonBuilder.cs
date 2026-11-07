using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Shipping.Core.Domain;
using Nop.Data.Mapping.Builders;

namespace Delsami.Plugin.Shipping.Core.Data.Mapping.Builder
{
    public class CartonBuilder : NopEntityBuilder<Carton>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Carton.Width)).AsDecimal().NotNullable()
                .WithColumn(nameof(Carton.Height)).AsDecimal().NotNullable()
                .WithColumn(nameof(Carton.Length)).AsDecimal().NotNullable()
                .WithColumn(nameof(Carton.Name)).AsString().NotNullable()
                .WithColumn(nameof(Carton.PostBoxNumber)).AsInt32().NotNullable();
        }
    }
}
