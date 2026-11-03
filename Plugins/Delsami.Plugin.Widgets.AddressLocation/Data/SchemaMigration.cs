using FluentMigrator;
using Delsami.Plugin.Widgets.AddressLocation.Domain;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace Delsami.Plugin.Widgets.AddressLocation.Data
{
    [NopMigration("2025/11/03 09:38:00", "Delsami.Plugin.Widgets.AddressLocation base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<StateProvinceLocation>();
            Create.TableFor<AddressCity>();
        }
    }
}
