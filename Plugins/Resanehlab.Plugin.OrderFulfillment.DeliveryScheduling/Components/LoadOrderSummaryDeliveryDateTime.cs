using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Common;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.UI;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Extensions;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Infrastructure;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Models;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Settings;
using System;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Components
{
    public class LoadOrderSummaryDeliveryDateTime : NopViewComponent
    {
        private readonly DeliverySchedulingSetting _deliverySchedulingSetting;
        private readonly IWorkContext _workContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly OrderSettings _orderSettings;

        public LoadOrderSummaryDeliveryDateTime(
            DeliverySchedulingSetting deliverySchedulingSetting,
            IWorkContext workContext,
            IGenericAttributeService genericAttributeService,
            OrderSettings orderSettings,
            INopHtmlHelper nopHtmlHelper)
        {
            _deliverySchedulingSetting = deliverySchedulingSetting;
            _workContext = workContext;
            _genericAttributeService = genericAttributeService;
            _orderSettings = orderSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {

            if (!_deliverySchedulingSetting.EnablePlugin)
                return Content(string.Empty);

            if (!(await _genericAttributeService.GetAttributeAsync<DateTime?>(await _workContext.GetCurrentCustomerAsync(), Constants.SystemCustomerAttributeNames_DeliveryDateOnUtc)).HasValue)
                return Content(string.Empty);

            var model = new OrderDetailsDeliveryDateTimeModel()
            {
                DeliveryDate = await (await _genericAttributeService.GetAttributeAsync<DateTime>(await _workContext.GetCurrentCustomerAsync(), Constants.SystemCustomerAttributeNames_DeliveryDateOnUtc)).FormatDateTimeAsync(),
                DeliveryTime = await _genericAttributeService.GetAttributeAsync<string>(await _workContext.GetCurrentCustomerAsync(), Constants.SystemCustomerAttributeNames_DeliveryTime),
                IsOpc = _orderSettings.OnePageCheckoutEnabled
            };

            return View("~/Plugins/Delsami.OrderFulfillment.DeliveryScheduling/Views/Shared/Components/LoadOrderSummaryDeliveryDateTime/Default.cshtml", model);
        }
    }
}

