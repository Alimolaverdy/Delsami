namespace Delsami.Plugin.Payment.Core.Enums
{
    public enum ErrorCode
    {
        OrderNotFound = 1,

        PaymentNotFound,

        GatewayError,

        GetTokenNotResponse,

        AmountValueNotEqual,

        StatusProblem,

        CanceledByUser
    }
}