using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Delsami.Plugin.Misc.SMS.Model;

namespace Delsami.Plugin.Misc.SMS.Data.Mapping.Builder
{
    internal class QueuedSMSBuilder : NopEntityBuilder<QueuedSMS>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                 .WithColumn(nameof(QueuedSMS.ReceiverNumber)).AsString().NotNullable()
                 .WithColumn(nameof(QueuedSMS.Body)).AsString(int.MaxValue).NotNullable()
                 .WithColumn(nameof(QueuedSMS.Result)).AsString(int.MaxValue).Nullable()
                 .WithColumn(nameof(QueuedSMS.CreatedDateTime)).AsDateTime2().NotNullable()
                 .WithColumn(nameof(QueuedSMS.SentDateTime)).AsDateTime2().Nullable()
                 .WithColumn(nameof(QueuedSMS.SentTries)).AsInt16().NotNullable()
                 .WithColumn(nameof(QueuedSMS.Status)).AsInt16().NotNullable();
        }
    }
}