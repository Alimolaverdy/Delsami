using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Models.Wallet;

namespace Delsami.Plugin.Misc.Core.Components
{
    public class CreditBalanceViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IRewardPointService _rewardPointService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;

        #endregion

        #region Ctor

        public CreditBalanceViewComponent(IWorkContext workContext, IStoreContext storeContext, IRewardPointService rewardPointService, IOrderTotalCalculationService orderTotalCalculationService, ICurrencyService currencyService, IPriceFormatter priceFormatter)
        {
            _workContext = workContext;
            _storeContext = storeContext;
            _rewardPointService = rewardPointService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _currencyService = currencyService;
            _priceFormatter = priceFormatter;
        }

        #endregion

        #region Methods

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            var currentStore = await _storeContext.GetCurrentStoreAsync();
            var rewardPointsBalance = await _rewardPointService.GetRewardPointsBalanceAsync(currentCustomer.Id, currentStore.Id);
            var rewardPointsAmountBase = await _orderTotalCalculationService.ConvertRewardPointsToAmountAsync(rewardPointsBalance);
            var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
            var rewardPointsAmount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(rewardPointsAmountBase, currentCurrency);

            var creditBalance = new CreditBalanceModel
            {
                Amount = await _priceFormatter.FormatPriceAsync(rewardPointsAmount, true, false)
            };

            return View("/Plugins/Delsami.Misc.Core/Views/Shared/Components/CreditBalance/Default.cshtml", creditBalance);
        }

        #endregion
    }
}
