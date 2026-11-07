namespace Delsami.Plugin.Payment.Core.Models;

public class PayResultModel : BasePaymentResponseModel
{
    public string TeminalNo { get; set; }

    public string ReferenceNumber { get; set; }

    public string TrackId { get; set; }

    public string MaskedCardNumber { get; set; }

    public string HashedCardNumber { get; set; }

    public string AdditionalData { get; set; }

    public string ResultCode { get; set; }

    public string Amount { get; set; }

    public override string ToString()
    {
        return $"TeminalNo: {TeminalNo} - ReferenceNumber: {ReferenceNumber} - TrackId: {TrackId} - MaskedCardNumber: {MaskedCardNumber} - HashedCardNumber: {HashedCardNumber} - ResultCode: {ResultCode} - Amount: {Amount} - AdditionalData: {AdditionalData}";
    }
}
