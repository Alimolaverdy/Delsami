using Delsami.Plugin.Payment.Core.Enums;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;

namespace Delsami.Plugin.Payment.Core.Areas.Admin.Models
{
    public record PaymentModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.OrderId")]
        public int OrderId { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.CustomerId")]
        public int CustomerId { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.SourceType")]
        public SourceType SourceType { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.Gateway")]
        public PaymentGateway Gateway { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.Gateway")]
        public string GatewayStr { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.TeminalNo")]
        public string TeminalNo { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.ReferenceNumber")]
        public string ReferenceNumber { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.TrackId")]
        public string TrackId { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.MaskedCardNumber")]
        public string MaskedCardNumber { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.HashedCardNumber")]
        public string HashedCardNumber { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.UrlId")]
        public string UrlId { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.Amount")]
        public decimal Amount { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.Amount")]
        public string AmountStr { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.AdditionalData")]
        public string AdditionalData { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.CreatedDateTime")]
        public DateTime CreatedDateTime { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.CreatedDateTime")]
        public string CreatedDateTimeStr { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.VerifyDateTime")]
        public DateTime? VerifyDateTime { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.VerifyDateTime")]
        public string VerifyDateTimeStr { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.ConfirmDateTime")]
        public DateTime? ConfirmDateTime { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.ConfirmDateTime")]
        public string ConfirmDateTimeStr { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.Status")]
        public PaymentStatus Status { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.Status")]
        public string StatusStr { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.ResultCode")]
        public string ResultCode { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Core.Admin.PaymentModel.Completed")]
        public bool Completed { get; set; }

    }
}
