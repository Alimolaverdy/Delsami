using System.ComponentModel;

namespace Delsami.Plugin.Payment.Core.Enums
{
    public enum PaymentStatus
    {
        [Description("Delsami.Plugin.Payment.Core.PaymentStatus.Confirmed")]
        Confirmed = 1,

        [Description("Delsami.Plugin.Payment.Core.PaymentStatus.Verified")]
        Verified,

        [Description("Delsami.Plugin.Payment.Core.PaymentStatus.Initial")]
        Initial,

        [Description("Delsami.Plugin.Payment.Core.PaymentStatus.InProgress")]
        InProgress,

        [Description("Delsami.Plugin.Payment.Core.PaymentStatus.Failed")]
        Failed,

        [Description("Delsami.Plugin.Payment.Core.PaymentStatus.Refunded")]
        Refunded
    }
}
