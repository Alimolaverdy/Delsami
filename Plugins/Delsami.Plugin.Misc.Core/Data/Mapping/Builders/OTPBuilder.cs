using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Domain;

namespace Delsami.Plugin.Misc.Core.Data.Mapping.Builders
{
    internal class OTPBuilder : NopEntityBuilder<OTP>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OTP.Username)).AsString().NotNullable()
                .WithColumn(nameof(OTP.Value)).AsString().NotNullable()
                .WithColumn(nameof(OTP.CreateDateUtc)).AsDateTime2().NotNullable()
                .WithColumn(nameof(OTP.ExpireOnUtc)).AsDateTime2().NotNullable()
                .WithColumn(nameof(OTP.Used)).AsBoolean().NotNullable()
                .WithColumn(nameof(OTP.TryCount)).AsInt32().NotNullable()
                .WithColumn(nameof(OTP.UsedDateUtc)).AsDateTime2().Nullable();
        }
    }
}