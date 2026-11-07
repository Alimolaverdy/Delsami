using FluentMigrator;
using Delsami.Plugin.Shipping.Core.Domain;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace Delsami.Plugin.Shipping.Core.Data
{
    [NopMigration("2026/11/07 18:38:00", "Delsami.Plugin.Misc.ShippingCore base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<Carton>();
            Create.TableFor<LogisticsScheduleException>();
            Create.TableFor<LogisticsSchedule>();
        }
    }
}
