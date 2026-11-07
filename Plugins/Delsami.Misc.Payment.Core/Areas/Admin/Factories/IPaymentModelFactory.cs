using Delsami.Plugin.Payment.Core.Areas.Admin.Models;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payment.Core.Areas.Admin.Factories
{
    public interface IPaymentModelFactory
    {
        PaymentSearchModel PreparePaymentSearchModel(PaymentSearchModel searchModel);

        Task<PaymentModel> PreparePaymentModelAsync(PaymentModel model, Domain.Payment payment);

        Task<PaymentListModel> PreparePaymentListModelAsync(PaymentSearchModel searchModel);
    }
}
