using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Extensions;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Infrastructure;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Settings;
using System;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Events
{
    public partial class EntityTokensAddedEventConsumer :
        IConsumer<EntityTokensAddedEvent<Order, Token>>
    {
        #region Methods

        private readonly ISettingService _settingService;
        private readonly IOrderShippingMethodCapacityMappingService _orderShippingMethodCapacityMappingService;
        private readonly ILocalizationService _localizationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IShippingMethodCapacityService _shippingMethodCapacityService;
        private readonly IAvailableDeliveryDateTimeRangeService _availableDeliveryDateTimeRangeService;
        private readonly IAvailableDeliveryTimeRangeService _availableDeliveryTimeRangeService;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public EntityTokensAddedEventConsumer(
            ISettingService settingService,
            IOrderShippingMethodCapacityMappingService orderShippingMethodCapacityMappingService,
            ILocalizationService localizationService,
            IGenericAttributeService genericAttributeService,
            IShippingMethodCapacityService shippingMethodCapacityService,
            IAvailableDeliveryDateTimeRangeService availableDeliveryDateTimeRangeService,
            IAvailableDeliveryTimeRangeService availableDeliveryTimeRangeService,
            ICustomerService customerService
            )
        {
            _settingService = settingService;
            _orderShippingMethodCapacityMappingService = orderShippingMethodCapacityMappingService;
            _localizationService = localizationService;
            _genericAttributeService = genericAttributeService;
            _shippingMethodCapacityService = shippingMethodCapacityService;
            _availableDeliveryDateTimeRangeService = availableDeliveryDateTimeRangeService;
            _availableDeliveryTimeRangeService = availableDeliveryTimeRangeService;
            _customerService = customerService;
        }

        #endregion

        #region Methods

        public async Task HandleEventAsync(EntityTokensAddedEvent<Order, Token> eventMessage)
        {
            if (eventMessage.Entity == null || eventMessage.Tokens == null)
                return;

            var deliverySchedulingSetting = await _settingService.LoadSettingAsync<DeliverySchedulingSetting>(eventMessage.Entity.StoreId);

            if (deliverySchedulingSetting.EnablePlugin)
            {
                var orderDeliveryDate = string.Empty;
                var orderDeliveryTime = string.Empty;

                var orderShippingMethodCapacityMapping = await _orderShippingMethodCapacityMappingService.GetOrderShippingMethodCapacityMappingByOrderIdAsync(eventMessage.Entity.Id);

                if (orderShippingMethodCapacityMapping != null)
                {
                    orderDeliveryDate = await orderShippingMethodCapacityMapping.DeliveryDateOnUtc.FormatDateTimeAsync();

                    var shippingMethodCapacity = await _shippingMethodCapacityService.GetShippingMethodCapacityByIdAsync(orderShippingMethodCapacityMapping.ShippingMethodCapacityId);

                    if (shippingMethodCapacity != null)
                    {
                        var availableDeliveryDateTimeRange = await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangeByIdAsync(shippingMethodCapacity.AvailableDeliveryDateTimeRangeId);

                        if (availableDeliveryDateTimeRange != null)
                        {
                            var availableDeliveryTimeRange = await _availableDeliveryTimeRangeService.GetAvailableDeliveryTimeRangeByIdAsync(availableDeliveryDateTimeRange.AvailableDeliveryTimeRangeId);

                            if (availableDeliveryTimeRange != null)
                                orderDeliveryTime = await _localizationService.GetLocalizedAsync(availableDeliveryTimeRange, d => d.Time);
                        }
                    }
                }
                else
                {
                    var customer = await _customerService.GetCustomerByIdAsync(eventMessage.Entity.CustomerId);

                    if (customer != null)
                    {
                        orderDeliveryDate = await (await _genericAttributeService.GetAttributeAsync<DateTime>(customer, Constants.SystemCustomerAttributeNames_DeliveryDateOnUtc)).FormatDateTimeAsync();
                        orderDeliveryTime = await _genericAttributeService.GetAttributeAsync<string>(customer, Constants.SystemCustomerAttributeNames_DeliveryTime);
                    }
                }

                eventMessage.Tokens.Add(new Token(Constants.OrderDeliveryDateMessageToken, orderDeliveryDate));
                eventMessage.Tokens.Add(new Token(Constants.OrderDeliveryTimeMessageToken, orderDeliveryTime));
            }
        }

        #endregion
    }
}