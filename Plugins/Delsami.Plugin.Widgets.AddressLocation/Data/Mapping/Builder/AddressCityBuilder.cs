using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Widgets.AddressLocation.Domain;
using Nop.Data.Mapping.Builders;

namespace Delsami.Plugin.Widgets.AddressLocation.Data.Mapping.Builder
{
    internal class AddressCityBuilder : NopEntityBuilder<AddressCity>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                 .WithColumn(nameof(AddressCity.AddressId)).AsInt16().NotNullable()
                 .WithColumn(nameof(AddressCity.CityId)).AsInt16().NotNullable();
        }
    }
}
