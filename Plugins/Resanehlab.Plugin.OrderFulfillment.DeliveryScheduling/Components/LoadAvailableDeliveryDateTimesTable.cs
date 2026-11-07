using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.UI;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Factories;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Infrastructure;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Models;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Settings;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Components
{
    public class LoadAvailableDeliveryDateTimesTable : NopViewComponent
    {
        private readonly DeliverySchedulingSetting _deliverySchedulingSetting;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IShippingService _shippingService;
        private readonly ILocalizationService _localizationService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICategoryService _categoryService;
        private readonly IOverriddenStartDisplayDaysOffsetService _overriddenStartDisplayDaysOffsetService;
        private readonly IAvailableDeliveryDateTimeTableModelFactory _availableDeliveryDateTimeTableModelFactory;

        public LoadAvailableDeliveryDateTimesTable(DeliverySchedulingSetting deliverySchedulingSetting,
            IWorkContext workContext,
            IStoreContext storeContext,
            IShippingService shippingService,
            ILocalizationService localizationService,
            IShoppingCartService shoppingCartService,
            INopHtmlHelper nopHtmlHelper,
            ICategoryService categoryService,
            IOverriddenStartDisplayDaysOffsetService overriddenStartDisplayDaysOffsetService,
            IAvailableDeliveryDateTimeTableModelFactory availableDeliveryDateTimeTableModelFactory)
        {
            _deliverySchedulingSetting = deliverySchedulingSetting;
            _workContext = workContext;
            _storeContext = storeContext;
            _shippingService = shippingService;
            _localizationService = localizationService;
            _shoppingCartService = shoppingCartService;
            _categoryService = categoryService;
            _overriddenStartDisplayDaysOffsetService = overriddenStartDisplayDaysOffsetService;
            _availableDeliveryDateTimeTableModelFactory = availableDeliveryDateTimeTableModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(string selectedShippingOption, bool isOpc)
        {
            if (!_deliverySchedulingSetting.EnablePlugin)
                return Content(string.Empty);

            if (string.IsNullOrEmpty(selectedShippingOption))
                throw new Exception("Selected shipping method can't be parsed");

            var splittedOption = selectedShippingOption.Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
            if (splittedOption.Length != 2)
                throw new Exception("Selected shipping method can't be parsed");

            string selectedName = splittedOption[0];

            var shippingMethod = await (await _shippingService.GetAllShippingMethodsAsync()).FirstOrDefaultAwaitAsync(async t => t.Name == selectedName || (await _localizationService.GetLocalizedAsync(t, x => x.Name)) == selectedName);

            if (shippingMethod == null)
                return Content(string.Empty);

            var model = new AvailableDeliveryDateTimeTableModel()
            {
                HideUnselectableDays = _deliverySchedulingSetting.HideUnselectableDays,
                IsOpc = isOpc,
                IsDisabled = _deliverySchedulingSetting.DisabledShippingMethodIdsAsInt.Contains(shippingMethod.Id)
            };

            if (!model.IsDisabled)
            {
                #region Calculate the first selectable day

                var startDisplayDaysOffset = _deliverySchedulingSetting.StartDisplayDaysOffset;

                var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStoreAsync()).Id);

                //get category Ids for cart products
                var cartProductsCategoriesIds = (await _categoryService.GetProductCategoryIdsAsync(cart.Select(t => t.ProductId).ToArray())).SelectMany(t => t.Value);

                var overriddenStartDisplayDaysOffsets = await _overriddenStartDisplayDaysOffsetService.GetAllOverriddenStartDisplayDaysOffsetsAsync(cartProductsCategoriesIds.ToArray());

                if (overriddenStartDisplayDaysOffsets.Any())
                    startDisplayDaysOffset = overriddenStartDisplayDaysOffsets.Max(t => t.StartDisplayDaysOffset);

                var firstSelectableDay = DateTime.Now.AddDays(startDisplayDaysOffset);

                #endregion

                #region Prepare AvailableDeliveryDateTimesTable

                model.AvailableDeliveryDateTimesTable = await _availableDeliveryDateTimeTableModelFactory.PrepareAvailableDeliveryDateTimesTableAsync(shippingMethod.Id, (await _storeContext.GetCurrentStoreAsync()).Id, (await _workContext.GetWorkingLanguageAsync()).Id, firstSelectableDay);

                #endregion
            }

            return View("~/Plugins/Delsami.OrderFulfillment.DeliveryScheduling/Views/Shared/Components/LoadAvailableDeliveryDateTimesTable/Default.cshtml", model);
        }
    }
}