using Delsami.Plugin.Misc.Core.Helpers;
using Delsami.Plugin.Payment.Core.Areas.Admin.Models;
using Delsami.Plugin.Payment.Core.Services;
using Nop.Core;
using Nop.Services.Catalog;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payment.Core.Areas.Admin.Factories
{
    public class PaymentModelFactory : IPaymentModelFactory
    {
        #region Fields

        private readonly IPaymentService _paymentService;
        private readonly IEnumHelper _enumHelper;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PaymentModelFactory(IPaymentService paymentService,
                                   IEnumHelper enumHelper,
                                   IPriceFormatter priceFormatter,
                                   IWorkContext workContext)
        {
            _paymentService = paymentService;
            _enumHelper = enumHelper;
            _priceFormatter = priceFormatter;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public virtual PaymentSearchModel PreparePaymentSearchModel(PaymentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public async Task<PaymentModel> PreparePaymentModelAsync(PaymentModel model, Domain.Payment payment)
        {
            if (model == null)
            {
                model = new PaymentModel();

                if (payment != null)
                {
                    model = payment.ToModel<PaymentModel>();
                    model.CreatedDateTimeStr = DateTimeHelper.GetPersianStrFromDateTime(payment.CreatedDateTime.ToLocalTime(), true);
                    
                    if (payment.VerifyDateTime.HasValue)
                        model.VerifyDateTimeStr = DateTimeHelper.GetPersianStrFromDateTime(payment.VerifyDateTime.Value.ToLocalTime(), true);

                    if (payment.ConfirmDateTime.HasValue)
                        model.ConfirmDateTimeStr = DateTimeHelper.GetPersianStrFromDateTime(payment.ConfirmDateTime.Value.ToLocalTime(), true);

                    model.StatusStr = await _enumHelper.GetDescriptionAsync(payment.Status);
                    model.GatewayStr = await _enumHelper.GetDescriptionAsync(payment.Gateway);

                    var cuurentCurrency = await _workContext.GetWorkingCurrencyAsync();

                    model.AmountStr = await _priceFormatter.FormatPriceAsync(payment.Amount, true, cuurentCurrency.CurrencyCode, false, (await _workContext.GetWorkingLanguageAsync()).Id);
                }
            }

            return model;
        }

        public virtual async Task<PaymentListModel> PreparePaymentListModelAsync(PaymentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var payments = await _paymentService.GetPagedListAsync(searchModel.OrderId,
                                                                   pageIndex: searchModel.Page - 1,
                                                                   pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new PaymentListModel().PrepareToGridAsync(searchModel, payments, () =>
            {
                return payments.SelectAwait(async payment =>
                {
                    var paymentModel = payment.ToModel<PaymentModel>();

                    //paymentModel.CustomerUsername = (await _customerService.GetCustomerByIdAsync(payment.CustomerId))?.Username;
                    paymentModel.CreatedDateTimeStr = DateTimeHelper.GetPersianStrFromDateTime(payment.CreatedDateTime.ToLocalTime(), true);

                    if (paymentModel.VerifyDateTime.HasValue)
                        paymentModel.VerifyDateTimeStr = DateTimeHelper.GetPersianStrFromDateTime(payment.VerifyDateTime.Value.ToLocalTime(), true);

                    if (paymentModel.ConfirmDateTime.HasValue)
                        paymentModel.ConfirmDateTimeStr = DateTimeHelper.GetPersianStrFromDateTime(payment.ConfirmDateTime.Value.ToLocalTime(), true);

                    paymentModel.StatusStr = await _enumHelper.GetDescriptionAsync(payment.Status);
                    paymentModel.GatewayStr = await _enumHelper.GetDescriptionAsync(payment.Gateway);

                    var cuurentCurrency = await _workContext.GetWorkingCurrencyAsync();

                    paymentModel.AmountStr = await _priceFormatter.FormatPriceAsync(payment.Amount, true, cuurentCurrency.CurrencyCode, false, (await _workContext.GetWorkingLanguageAsync()).Id);

                    return paymentModel;
                });
            });

            return model;
        }

        #endregion
    }
}
