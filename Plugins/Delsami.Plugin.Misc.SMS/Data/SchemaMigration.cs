using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Delsami.Plugin.Misc.SMS.Model;

namespace Delsami.Plugin.Misc.SMS.Data
{

    [NopMigration("2025/11/03 13:00:00", "Delsami.Plugin.Misc.SMS base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<QueuedSMS>();
        }
    }
}
