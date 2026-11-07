using Delsami.Plugin.Payment.Core.Enums;
using Nop.Core;
using System;

namespace Delsami.Plugin.Payment.Core.Domain
{
    public class Payment : BaseEntity
    {
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public PaymentType PaymentType { get; set; }

        public SourceType SourceType { get; set; }

        public PaymentGateway Gateway { get; set; }

        public string TeminalNo { get; set; }

        public string ReferenceNumber { get; set; }

        public string TrackId { get; set; }

        public string MaskedCardNumber { get; set; }

        public string HashedCardNumber { get; set; }

        public string UrlId { get; set; }

        public decimal Amount { get; set; }

        public string AdditionalData { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime? VerifyDateTime { get; set; }

        public DateTime? ConfirmDateTime { get; set; }

        public PaymentStatus Status { get; set; }

        public bool CanceledByUser { get; set; }

        public string ResultCode { get; set; }

        public bool Completed { get; set; }

    }
}
