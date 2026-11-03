using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Widgets.AddressLocation.Domain;
using Nop.Data.Mapping.Builders;

namespace Delsami.Plugin.Widgets.AddressLocation.Data.Mapping.Builder
{
    public class StateProvinceLocationBuilder : NopEntityBuilder<StateProvinceLocation>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                 .WithColumn(nameof(StateProvinceLocation.StateProvinceId)).AsInt16().NotNullable()
                 .WithColumn(nameof(StateProvinceLocation.Latitude)).AsDecimal().NotNullable()
                 .WithColumn(nameof(StateProvinceLocation.Longitude)).AsDecimal().NotNullable();
        }
    }
}
