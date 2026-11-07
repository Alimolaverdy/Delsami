using System.ComponentModel;

namespace Delsami.Plugin.Payment.Core.Enums
{
    public enum PaymentGateway
    {
        [Description("Delsami.Plugin.Payment.Core.PaymentGateway.Pasargad")]
        Pasargad = 1,

        [Description("Delsami.Plugin.Payment.Core.PaymentGateway.Tejarat")]
        Tejarat,

        [Description("Delsami.Plugin.Payment.Core.PaymentGateway.Mellat")]
        Mellat,

        [Description("Delsami.Plugin.Payment.Core.PaymentGateway.Torob")]
        Torob,

        [Description("Delsami.Plugin.Payment.Core.PaymentGateway.Saman")]
        Saman,

        [Description("Delsami.Plugin.Payment.Core.PaymentGateway.Resalat")]
        Resalat,

        [Description("Delsami.Plugin.Payment.Core.PaymentGateway.Digipay")]
        Digipay,

        [Description("Delsami.Plugin.Payment.Core.PaymentGateway.Tara")]
        Tara,
        
        [Description("Delsami.Plugin.Payment.Core.PaymentGateway.DigiKala")]
        DigiKala,
        
        [Description("Delsami.Plugin.Payment.Core.PaymentGateway.Barook")]
        Barook,

        [Description("Delsami.Plugin.Payment.Core.PaymentGateway.Parsian")]
        Parsian
    }
}
