using FluentMigrator;
using Delsami.Plugin.Shipping.DelsamiLogistic.Domain;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace Delsami.Plugin.Shipping.DelsamiLogistic.Data
{
    [NopMigration("2026/11/07 12:00:00", "Delsami.Plugin.Shipping.DelsamiLogistic base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<QueuedCanceledShipmentForSendToLogistic>();
        }
    }
}
