using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using PaymentDomain = Delsami.Plugin.Payment.Core.Domain;

namespace Delsami.Plugin.Payment.Core.Data
{
    [NopMigration("2023/11/07 14:38:00", "Delsami.Plugin.Payment.Core base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<PaymentDomain.Payment>();
        }
    }
}
