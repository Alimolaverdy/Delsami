using DocumentFormat.OpenXml.Bibliography;
using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Delsami.Plugin.Misc.Core.Domain;
using City = Delsami.Plugin.Misc.Core.Domain.City;

namespace Delsami.Plugin.Misc.Core.Data
{
    [NopMigration("2025/11/03 14:35:20", "Delsami.Plugin.Misc.Core base schema")]
    internal class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<City>();
            Create.TableFor<OTP>();
        }
    }
}
