using System.ComponentModel;

namespace Delsami.Plugin.Payment.Core.Enums
{
    public enum PaymentType
    {
        [Description("پرداخت اینترنتی")]
        IPG = 1,

        [Description("پرداخت در محل")]
        COD,

        [Description("پرداخت قسطی")]
        CPG,

        //[Description("کیف پول")]
        //Wallet

    }
}
