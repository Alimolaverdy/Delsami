using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using PaymentDomain = Delsami.Plugin.Payment.Core.Domain;

namespace Delsami.Plugin.Payment.Core.Data.Mapping.Builder
{
    public class PaymentBuilder : NopEntityBuilder<PaymentDomain.Payment>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PaymentDomain.Payment.OrderId)).AsInt32().NotNullable()
                .WithColumn(nameof(PaymentDomain.Payment.CustomerId)).AsInt32().NotNullable()
                .WithColumn(nameof(PaymentDomain.Payment.SourceType)).AsInt32().NotNullable()
                .WithColumn(nameof(PaymentDomain.Payment.Gateway)).AsInt32().NotNullable()
                .WithColumn(nameof(PaymentDomain.Payment.TeminalNo)).AsString().NotNullable()
                .WithColumn(nameof(PaymentDomain.Payment.AdditionalData)).AsString().Nullable()
                .WithColumn(nameof(PaymentDomain.Payment.VerifyDateTime)).AsDateTime2().Nullable()
                .WithColumn(nameof(PaymentDomain.Payment.CreatedDateTime)).AsDateTime2().NotNullable()
                .WithColumn(nameof(PaymentDomain.Payment.ConfirmDateTime)).AsDateTime2().Nullable()
                .WithColumn(nameof(PaymentDomain.Payment.Amount)).AsDecimal().NotNullable()
                .WithColumn(nameof(PaymentDomain.Payment.HashedCardNumber)).AsString().Nullable()
                .WithColumn(nameof(PaymentDomain.Payment.MaskedCardNumber)).AsString().Nullable()
                .WithColumn(nameof(PaymentDomain.Payment.UrlId)).AsString().Nullable()
                .WithColumn(nameof(PaymentDomain.Payment.ReferenceNumber)).AsString().Nullable()
                .WithColumn(nameof(PaymentDomain.Payment.TrackId)).AsString().Nullable()
                .WithColumn(nameof(PaymentDomain.Payment.Status)).AsInt16().NotNullable()
                .WithColumn(nameof(PaymentDomain.Payment.CanceledByUser)).AsBoolean().NotNullable()
                .WithColumn(nameof(PaymentDomain.Payment.ResultCode)).AsString().Nullable()
                .WithColumn(nameof(PaymentDomain.Payment.PaymentType)).AsInt32().NotNullable()
                .WithColumn(nameof(PaymentDomain.Payment.Completed)).AsInt32().Nullable();
        }
    }
}
