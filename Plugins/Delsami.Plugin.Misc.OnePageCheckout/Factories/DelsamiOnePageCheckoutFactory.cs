//using Delsami.Plugin.Misc.OnePageCheckout.Models;
//using Nop.Core;
//using Nop.Core.Domain.Common;
//using Nop.Core.Domain.Customers;
//using Nop.Core.Domain.Directory;
//using Nop.Core.Domain.Orders;
//using Nop.Core.Domain.Payments;
//using Nop.Core.Domain.Security;
//using Nop.Core.Domain.Shipping;
//using Nop.Core.Domain.Tax;
//using Nop.Services.Catalog;
//using Nop.Services.Common;
//using Nop.Services.Customers;
//using Nop.Services.Directory;
//using Nop.Services.Localization;
//using Nop.Services.Orders;
//using Nop.Services.Payments;
//using Nop.Services.Shipping;
//using Nop.Services.Shipping.Pickup;
//using Nop.Services.Stores;
//using Nop.Services.Tax;
//using Nop.Web.Factories;
//using Nop.Web.Models.Checkout;
//using Nop.Web.Models.Common;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Delsami.Plugin.Misc.OnePageCheckout.Factories
//{
//    public class DelsamiOnePageCheckoutFactory : IDelsamiOnePageCheckoutFactory
//    {
//        #region Fields

//        private readonly IWorkContext _workContext;
//        private readonly IShoppingCartService _shoppingCartService;
//        private readonly OrderSettings _orderSettings;
//        private readonly ICustomerService _customerService;
//        private readonly CaptchaSettings _captchaSettings;
//        private readonly ShippingSettings _shippingSettings;
//        private readonly TaxSettings _taxSettings;
//        private readonly ICountryService _countryService;
//        private readonly IStoreMappingService _storeMappingService;
//        private readonly IAddressModelFactory _addressModelFactory;
//        private readonly AddressSettings _addressSettings;
//        private readonly IAddressService _addressService;
//        private readonly IStoreContext _storeContext;
//        private readonly IShippingService _shippingService;
//        private readonly IGenericAttributeService _genericAttributeService;
//        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
//        private readonly ITaxService _taxService;
//        private readonly ICurrencyService _currencyService;
//        private readonly RewardPointsSettings _rewardPointsSettings;
//        private readonly IPriceFormatter _priceFormatter;
//        private readonly IRewardPointService _rewardPointService;
//        private readonly IOrderProcessingService _orderProcessingService;
//        private readonly IPaymentPluginManager _paymentPluginManager;
//        private readonly ILocalizationService _localizationService;
//        private readonly PaymentSettings _paymentSettings;
//        private readonly CommonSettings _commonSettings;
//        private readonly IPaymentService _paymentService;
//        private readonly IPickupPluginManager _pickupPluginManager;
//        private readonly IStateProvinceService _stateProvinceService;
//        private readonly IShippingPluginManager _shippingPluginManager;

//        #endregion

//        #region Ctor

//        public DelsamiOnePageCheckoutFactory(IWorkContext workContext,
//                                              IShoppingCartService shoppingCartService,
//                                              OrderSettings orderSettings,
//                                              ICustomerService customerService,
//                                              CaptchaSettings captchaSettings,
//                                              ShippingSettings shippingSettings,
//                                              TaxSettings taxSettings,
//                                              ICountryService countryService,
//                                              IStoreMappingService storeMappingService,
//                                              IAddressModelFactory addressModelFactory,
//                                              AddressSettings addressSettings,
//                                              IAddressService addressService,
//                                              IStoreContext storeContext,
//                                              IShippingService shippingService,
//                                              IGenericAttributeService genericAttributeService,
//                                              IOrderTotalCalculationService orderTotalCalculationService,
//                                              ITaxService taxService,
//                                              ICurrencyService currencyService,
//                                              RewardPointsSettings rewardPointsSettings,
//                                              IPriceFormatter priceFormatter,
//                                              IRewardPointService rewardPointService,
//                                              IOrderProcessingService orderProcessingService,
//                                              IPaymentPluginManager paymentPluginManager,
//                                              ILocalizationService localizationService,
//                                              PaymentSettings paymentSettings,
//                                              CommonSettings commonSettings,
//                                              IPaymentService paymentService,
//                                              IPickupPluginManager pickupPluginManager,
//                                              IStateProvinceService stateProvinceService,
//                                              IShippingPluginManager shippingPluginManager)
//        {
//            _workContext = workContext;
//            _shoppingCartService = shoppingCartService;
//            _orderSettings = orderSettings;
//            _customerService = customerService;
//            _captchaSettings = captchaSettings;
//            _shippingSettings = shippingSettings;
//            _taxSettings = taxSettings;
//            _countryService = countryService;
//            _storeMappingService = storeMappingService;
//            _addressModelFactory = addressModelFactory;
//            _addressSettings = addressSettings;
//            _addressService = addressService;
//            _storeContext = storeContext;
//            _shippingService = shippingService;
//            _genericAttributeService = genericAttributeService;
//            _orderTotalCalculationService = orderTotalCalculationService;
//            _taxService = taxService;
//            _currencyService = currencyService;
//            _rewardPointsSettings = rewardPointsSettings;
//            _priceFormatter = priceFormatter;
//            _rewardPointService = rewardPointService;
//            _orderProcessingService = orderProcessingService;
//            _paymentPluginManager = paymentPluginManager;
//            _localizationService = localizationService;
//            _paymentSettings = paymentSettings;
//            _commonSettings = commonSettings;
//            _paymentService = paymentService;
//            _pickupPluginManager = pickupPluginManager;
//            _stateProvinceService = stateProvinceService;
//            _shippingPluginManager = shippingPluginManager;
//        }

//        #endregion

//        #region Methods

//        public virtual async Task<DelsamiOnePageCheckoutModel> PrepareOnePageCheckoutModelAsync(IList<ShoppingCartItem> cart)
//        {
//            if (cart == null)
//                throw new ArgumentNullException(nameof(cart));

//            var customer = await _workContext.GetCurrentCustomerAsync();

//            var filterByCountryId = 0;
//            if (_addressSettings.CountryEnabled)
//            {
//                filterByCountryId = (await _customerService.GetCustomerBillingAddressAsync(customer))?.CountryId ?? 0;
//            }

//            var model = new DelsamiOnePageCheckoutModel
//            {
//                ShippingRequired = await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart),
//                DisableBillingAddressCheckoutStep = _orderSettings.DisableBillingAddressCheckoutStep && (await _customerService.GetAddressesByCustomerIdAsync(customer.Id)).Any(),
//                DisplayCaptcha = await _customerService.IsGuestAsync(await _customerService.GetShoppingCartCustomerAsync(cart))
//                    && _captchaSettings.Enabled && _captchaSettings.ShowOnCheckoutPageForGuests,
//                IsReCaptchaV3 = _captchaSettings.CaptchaType == CaptchaType.ReCaptchaV3,
//                ReCaptchaPublicKey = _captchaSettings.ReCaptchaPublicKey,
//                CheckoutBillingAddress = await PrepareBillingAddressModelAsync(cart, prePopulateNewAddressWithCustomerFields: true),
//                CheckoutShippingAddress = await PrepareShippingAddressModelAsync(cart, prePopulateNewAddressWithCustomerFields: true),
//                CheckoutShippingMethod = await PrepareShippingMethodModelAsync(cart, await _customerService.GetCustomerShippingAddressAsync(customer)),
//                CheckoutPaymentMethod = await PreparePaymentMethodModelAsync(cart, filterByCountryId),
//                //CheckoutPaymentInfo= PreparePaymentInfoModelAsync(paymentMethod)
//            };

//            return model;
//        }

//        #endregion

//        #region Utilities

//        /// <summary>
//        /// Prepare billing address model
//        /// </summary>
//        /// <param name="cart">Cart</param>
//        /// <param name="selectedCountryId">Selected country identifier</param>
//        /// <param name="prePopulateNewAddressWithCustomerFields">Pre populate new address with customer fields</param>
//        /// <param name="overrideAttributesXml">Override attributes xml</param>
//        /// <returns>
//        /// A task that represents the asynchronous operation
//        /// The task result contains the billing address model
//        /// </returns>
//        public virtual async Task<CheckoutBillingAddressModel> PrepareBillingAddressModelAsync(IList<ShoppingCartItem> cart,
//            int? selectedCountryId = null,
//        bool prePopulateNewAddressWithCustomerFields = false,
//            string overrideAttributesXml = "")
//        {
//            var model = new CheckoutBillingAddressModel
//            {
//                ShipToSameAddressAllowed = _shippingSettings.ShipToSameAddress && await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart),
//                //allow customers to enter (choose) a shipping address if "Disable Billing address step" setting is enabled
//                ShipToSameAddress = !_orderSettings.DisableBillingAddressCheckoutStep
//            };

//            var customer = await _workContext.GetCurrentCustomerAsync();
//            if (await _customerService.IsGuestAsync(customer) && _taxSettings.EuVatEnabled)
//            {
//                model.VatNumber = customer.VatNumber;
//                model.EuVatEnabled = true;
//                model.EuVatEnabledForGuests = _taxSettings.EuVatEnabledForGuests;
//            }

//            //existing addresses
//            var addresses = await (await _customerService.GetAddressesByCustomerIdAsync(customer.Id))
//                .WhereAwait(async a => !a.CountryId.HasValue || await _countryService.GetCountryByAddressAsync(a) is Country country &&
//                    (//published
//                    country.Published &&
//                    //allow billing
//                    country.AllowsBilling &&
//                    //enabled for the current store
//                    await _storeMappingService.AuthorizeAsync(country)))
//                .ToListAsync();
//            foreach (var address in addresses)
//            {
//                var addressModel = new AddressModel();
//                await _addressModelFactory.PrepareAddressModelAsync(addressModel,
//                    address: address,
//                    excludeProperties: false,
//                    addressSettings: _addressSettings);

//                if (await _addressService.IsAddressValidAsync(address))
//                {
//                    model.ExistingAddresses.Add(addressModel);
//                }
//                else
//                {
//                    model.InvalidExistingAddresses.Add(addressModel);
//                }
//            }

//            //new address
//            model.BillingNewAddress.CountryId = selectedCountryId;
//            await _addressModelFactory.PrepareAddressModelAsync(model.BillingNewAddress,
//                address: null,
//                excludeProperties: false,
//                addressSettings: _addressSettings,
//                loadCountries: async () => await _countryService.GetAllCountriesForBillingAsync((await _workContext.GetWorkingLanguageAsync()).Id),
//                prePopulateWithCustomerFields: prePopulateNewAddressWithCustomerFields,
//                customer: customer,
//                overrideAttributesXml: overrideAttributesXml);

//            return model;
//        }

//        /// <summary>
//        /// Prepare shipping address model
//        /// </summary>
//        /// <param name="cart">Cart</param>
//        /// <param name="selectedCountryId">Selected country identifier</param>
//        /// <param name="prePopulateNewAddressWithCustomerFields">Pre populate new address with customer fields</param>
//        /// <param name="overrideAttributesXml">Override attributes xml</param>
//        /// <returns>
//        /// A task that represents the asynchronous operation
//        /// The task result contains the shipping address model
//        /// </returns>
//        public virtual async Task<CheckoutShippingAddressModel> PrepareShippingAddressModelAsync(IList<ShoppingCartItem> cart,
//            int? selectedCountryId = null, bool prePopulateNewAddressWithCustomerFields = false, string overrideAttributesXml = "")
//        {
//            var model = new CheckoutShippingAddressModel
//            {
//                DisplayPickupInStore = !_orderSettings.DisplayPickupInStoreOnShippingMethodPage
//            };

//            if (!_orderSettings.DisplayPickupInStoreOnShippingMethodPage)
//                model.PickupPointsModel = await PrepareCheckoutPickupPointsModelAsync(cart);

//            //existing addresses
//            var customer = await _workContext.GetCurrentCustomerAsync();
//            var addresses = await (await _customerService.GetAddressesByCustomerIdAsync(customer.Id))
//                .WhereAwait(async a => !a.CountryId.HasValue || await _countryService.GetCountryByAddressAsync(a) is Country country &&
//                    (//published
//                    country.Published &&
//                    //allow shipping
//                    country.AllowsShipping &&
//                    //enabled for the current store
//                    await _storeMappingService.AuthorizeAsync(country)))
//                .ToListAsync();
//            foreach (var address in addresses)
//            {
//                var addressModel = new AddressModel();
//                await _addressModelFactory.PrepareAddressModelAsync(addressModel,
//                    address: address,
//                    excludeProperties: false,
//                    addressSettings: _addressSettings);

//                if (await _addressService.IsAddressValidAsync(address))
//                {
//                    model.ExistingAddresses.Add(addressModel);
//                }
//                else
//                {
//                    model.InvalidExistingAddresses.Add(addressModel);
//                }
//            }

//            //new address
//            model.ShippingNewAddress.CountryId = selectedCountryId;
//            await _addressModelFactory.PrepareAddressModelAsync(model.ShippingNewAddress,
//                address: null,
//                excludeProperties: false,
//                addressSettings: _addressSettings,
//                loadCountries: async () => await _countryService.GetAllCountriesForShippingAsync((await _workContext.GetWorkingLanguageAsync()).Id),
//                prePopulateWithCustomerFields: prePopulateNewAddressWithCustomerFields,
//                customer: customer,
//                overrideAttributesXml: overrideAttributesXml);

//            model.SelectedBillingAddress = customer.BillingAddressId ?? 0;

//            return model;
//        }

//        /// <summary>
//        /// Prepare shipping method model
//        /// </summary>
//        /// <param name="cart">Cart</param>
//        /// <param name="shippingAddress">Shipping address</param>
//        /// <returns>
//        /// A task that represents the asynchronous operation
//        /// The task result contains the shipping method model
//        /// </returns>
//        public virtual async Task<CheckoutShippingMethodModel> PrepareShippingMethodModelAsync(IList<ShoppingCartItem> cart, Address shippingAddress)
//        {
//            var model = new CheckoutShippingMethodModel
//            {
//                DisplayPickupInStore = _orderSettings.DisplayPickupInStoreOnShippingMethodPage
//            };

//            if (_orderSettings.DisplayPickupInStoreOnShippingMethodPage)
//                model.PickupPointsModel = await PrepareCheckoutPickupPointsModelAsync(cart);

//            var customer = await _workContext.GetCurrentCustomerAsync();
//            var store = await _storeContext.GetCurrentStoreAsync();
//            var getShippingOptionResponse = await _shippingService.GetShippingOptionsAsync(cart, shippingAddress, customer, storeId: store.Id);
//            if (getShippingOptionResponse.Success)
//            {
//                //performance optimization. cache returned shipping options.
//                //we'll use them later (after a customer has selected an option).
//                await _genericAttributeService.SaveAttributeAsync(customer,
//                                                       NopCustomerDefaults.OfferedShippingOptionsAttribute,
//                                                       getShippingOptionResponse.ShippingOptions,
//                                                       store.Id);

//                foreach (var shippingOption in getShippingOptionResponse.ShippingOptions)
//                {
//                    var soModel = new CheckoutShippingMethodModel.ShippingMethodModel
//                    {
//                        Name = shippingOption.Name,
//                        Description = shippingOption.Description,
//                        DisplayOrder = shippingOption.DisplayOrder ?? 0,
//                        ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName,
//                        ShippingOption = shippingOption,
//                    };

//                    //adjust rate
//                    var (shippingTotal, _) = await _orderTotalCalculationService.AdjustShippingRateAsync(shippingOption.Rate, cart, shippingOption.IsPickupInStore);

//                    var (rateBase, _) = await _taxService.GetShippingPriceAsync(shippingTotal, customer);
//                    var rate = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(rateBase, await _workContext.GetWorkingCurrencyAsync());
//                    soModel.Fee = await _priceFormatter.FormatShippingPriceAsync(rate, true);
//                    soModel.Rate = rate;
//                    model.ShippingMethods.Add(soModel);
//                }

//                //sort shipping methods
//                if (model.ShippingMethods.Count > 1)
//                {
//                    model.ShippingMethods = (_shippingSettings.ShippingSorting switch
//                    {
//                        ShippingSortingEnum.ShippingCost => model.ShippingMethods.OrderBy(option => option.Rate),
//                        _ => model.ShippingMethods.OrderBy(option => option.DisplayOrder)
//                    }).ToList();
//                }

//                //find a selected (previously) shipping method
//                var selectedShippingOption = await _genericAttributeService.GetAttributeAsync<ShippingOption>(customer,
//                        NopCustomerDefaults.SelectedShippingOptionAttribute, store.Id);
//                if (selectedShippingOption != null)
//                {
//                    var shippingOptionToSelect = model.ShippingMethods.ToList()
//                        .Find(so =>
//                           !string.IsNullOrEmpty(so.Name) &&
//                           so.Name.Equals(selectedShippingOption.Name, StringComparison.InvariantCultureIgnoreCase) &&
//                           !string.IsNullOrEmpty(so.ShippingRateComputationMethodSystemName) &&
//                           so.ShippingRateComputationMethodSystemName.Equals(selectedShippingOption.ShippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase));
//                    if (shippingOptionToSelect != null)
//                    {
//                        shippingOptionToSelect.Selected = true;
//                    }
//                }
//                //if no option has been selected, let's do it for the first one
//                if (model.ShippingMethods.FirstOrDefault(so => so.Selected) == null)
//                {
//                    var shippingOptionToSelect = model.ShippingMethods.FirstOrDefault();
//                    if (shippingOptionToSelect != null)
//                    {
//                        shippingOptionToSelect.Selected = true;
//                    }
//                }

//                //notify about shipping from multiple locations
//                if (_shippingSettings.NotifyCustomerAboutShippingFromMultipleLocations)
//                {
//                    model.NotifyCustomerAboutShippingFromMultipleLocations = getShippingOptionResponse.ShippingFromMultipleLocations;
//                }
//            }
//            else
//            {
//                foreach (var error in getShippingOptionResponse.Errors)
//                    model.Warnings.Add(error);
//            }

//            return model;
//        }

//        /// <summary>
//        /// Prepare payment method model
//        /// </summary>
//        /// <param name="cart">Cart</param>
//        /// <param name="filterByCountryId">Filter by country identifier</param>
//        /// <returns>
//        /// A task that represents the asynchronous operation
//        /// The task result contains the payment method model
//        /// </returns>
//        public virtual async Task<CheckoutPaymentMethodModel> PreparePaymentMethodModelAsync(IList<ShoppingCartItem> cart, int filterByCountryId)
//        {
//            var model = new CheckoutPaymentMethodModel();

//            var customer = await _workContext.GetCurrentCustomerAsync();
//            var store = await _storeContext.GetCurrentStoreAsync();

//            //reward points
//            if (_rewardPointsSettings.Enabled && !await _shoppingCartService.ShoppingCartIsRecurringAsync(cart))
//            {
//                var shoppingCartTotal = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart, true, false);
//                if (shoppingCartTotal.redeemedRewardPoints > 0)
//                {
//                    model.DisplayRewardPoints = true;
//                    model.RewardPointsToUseAmount = await _priceFormatter.FormatPriceAsync(shoppingCartTotal.redeemedRewardPointsAmount, true, false);
//                    model.RewardPointsToUse = shoppingCartTotal.redeemedRewardPoints;
//                    model.RewardPointsBalance = await _rewardPointService.GetRewardPointsBalanceAsync(customer.Id, store.Id);

//                    //are points enough to pay for entire order? like if this option (to use them) was selected
//                    model.RewardPointsEnoughToPayForOrder = !await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart, true);
//                }
//            }

//            //filter by country
//            var paymentMethods = await (await _paymentPluginManager
//                .LoadActivePluginsAsync(customer, store.Id, filterByCountryId))
//                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)
//                .WhereAwait(async pm => !await pm.HidePaymentMethodAsync(cart))
//                .ToListAsync();
//            foreach (var pm in paymentMethods)
//            {
//                if (await _shoppingCartService.ShoppingCartIsRecurringAsync(cart) && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
//                    continue;

//                var pmModel = new CheckoutPaymentMethodModel.PaymentMethodModel
//                {
//                    Name = await _localizationService.GetLocalizedFriendlyNameAsync(pm, (await _workContext.GetWorkingLanguageAsync()).Id),
//                    Description = _paymentSettings.ShowPaymentMethodDescriptions ? await pm.GetPaymentMethodDescriptionAsync() : string.Empty,
//                    PaymentMethodSystemName = pm.PluginDescriptor.SystemName,
//                    LogoUrl = await _paymentPluginManager.GetPluginLogoUrlAsync(pm)
//                };
//                //payment method additional fee
//                var paymentMethodAdditionalFee = await _paymentService.GetAdditionalHandlingFeeAsync(cart, pm.PluginDescriptor.SystemName);
//                var (rateBase, _) = await _taxService.GetPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFee, customer);
//                var rate = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(rateBase, await _workContext.GetWorkingCurrencyAsync());

//                if (rate > decimal.Zero)
//                    pmModel.Fee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(rate, true);

//                model.PaymentMethods.Add(pmModel);
//            }

//            //find a selected (previously) payment method
//            var selectedPaymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer,
//                NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
//            if (!string.IsNullOrEmpty(selectedPaymentMethodSystemName))
//            {
//                var paymentMethodToSelect = model.PaymentMethods.ToList()
//                    .Find(pm => pm.PaymentMethodSystemName.Equals(selectedPaymentMethodSystemName, StringComparison.InvariantCultureIgnoreCase));
//                if (paymentMethodToSelect != null)
//                    paymentMethodToSelect.Selected = true;
//            }
//            //if no option has been selected, let's do it for the first one
//            if (model.PaymentMethods.FirstOrDefault(so => so.Selected) == null)
//            {
//                var paymentMethodToSelect = model.PaymentMethods.FirstOrDefault();
//                if (paymentMethodToSelect != null)
//                    paymentMethodToSelect.Selected = true;
//            }

//            return model;
//        }

//        /// <summary>
//        /// Prepare payment info model
//        /// </summary>
//        /// <param name="paymentMethod">Payment method</param>
//        /// <returns>
//        /// A task that represents the asynchronous operation
//        /// The task result contains the payment info model
//        /// </returns>
//        public virtual Task<CheckoutPaymentInfoModel> PreparePaymentInfoModelAsync(IPaymentMethod paymentMethod)
//        {
//            return Task.FromResult(new CheckoutPaymentInfoModel
//            {
//                PaymentViewComponent = paymentMethod.GetPublicViewComponent(),
//                DisplayOrderTotals = _orderSettings.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab
//            });
//        }

//        /// <summary>
//        /// Prepare confirm order model
//        /// </summary>
//        /// <param name="cart">Cart</param>
//        /// <returns>
//        /// A task that represents the asynchronous operation
//        /// The task result contains the confirm order model
//        /// </returns>
//        public virtual async Task<CheckoutConfirmModel> PrepareConfirmOrderModelAsync(IList<ShoppingCartItem> cart)
//        {
//            var model = new CheckoutConfirmModel
//            {
//                //terms of service
//                TermsOfServiceOnOrderConfirmPage = _orderSettings.TermsOfServiceOnOrderConfirmPage,
//                TermsOfServicePopup = _commonSettings.PopupForTermsOfServiceLinks,
//                DisplayCaptcha = await _customerService.IsGuestAsync(await _customerService.GetShoppingCartCustomerAsync(cart))
//                    && _captchaSettings.Enabled && _captchaSettings.ShowOnCheckoutPageForGuests
//            };
//            //min order amount validation
//            var minOrderTotalAmountOk = await _orderProcessingService.ValidateMinOrderTotalAmountAsync(cart);
//            if (!minOrderTotalAmountOk)
//            {
//                var minOrderTotalAmount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(_orderSettings.MinOrderTotalAmount, await _workContext.GetWorkingCurrencyAsync());
//                model.MinOrderTotalWarning = string.Format(await _localizationService.GetResourceAsync("Checkout.MinOrderTotalAmount"), await _priceFormatter.FormatPriceAsync(minOrderTotalAmount, true, false));
//            }
//            return model;
//        }

//        /// <summary>
//        /// Prepare checkout completed model
//        /// </summary>
//        /// <param name="order">Order</param>
//        /// <returns>
//        /// A task that represents the asynchronous operation
//        /// The task result contains the checkout completed model
//        /// </returns>
//        public virtual Task<CheckoutCompletedModel> PrepareCheckoutCompletedModelAsync(Order order)
//        {
//            if (order == null)
//                throw new ArgumentNullException(nameof(order));

//            var model = new CheckoutCompletedModel
//            {
//                OrderId = order.Id,
//                OnePageCheckoutEnabled = _orderSettings.OnePageCheckoutEnabled,
//                CustomOrderNumber = order.CustomOrderNumber
//            };

//            return Task.FromResult(model);
//        }

//        #endregion

//        #region Utilities

//        /// Prepares the checkout pickup points model
//        /// </summary>
//        /// <param name="cart">Cart</param>
//        /// <returns>
//        /// A task that represents the asynchronous operation
//        /// The task result contains the checkout pickup points model
//        /// </returns>
//        protected virtual async Task<CheckoutPickupPointsModel> PrepareCheckoutPickupPointsModelAsync(IList<ShoppingCartItem> cart)
//        {
//            var model = new CheckoutPickupPointsModel
//            {
//                AllowPickupInStore = _shippingSettings.AllowPickupInStore
//            };

//            if (!model.AllowPickupInStore)
//                return model;

//            model.DisplayPickupPointsOnMap = _shippingSettings.DisplayPickupPointsOnMap;
//            model.GoogleMapsApiKey = _shippingSettings.GoogleMapsApiKey;
//            var customer = await _workContext.GetCurrentCustomerAsync();
//            var store = await _storeContext.GetCurrentStoreAsync();
//            var pickupPointProviders = await _pickupPluginManager.LoadActivePluginsAsync(customer, store.Id);
//            if (pickupPointProviders.Any())
//            {
//                var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
//                var address = customer.BillingAddressId.HasValue
//                    ? await _addressService.GetAddressByIdAsync(customer.BillingAddressId.Value)
//                    : null;
//                var pickupPointsResponse = await _shippingService.GetPickupPointsAsync(cart, address,
//                    customer, storeId: store.Id);
//                if (pickupPointsResponse.Success)
//                    model.PickupPoints = await pickupPointsResponse.PickupPoints.SelectAwait(async point =>
//                    {
//                        var country = await _countryService.GetCountryByTwoLetterIsoCodeAsync(point.CountryCode);
//                        var state = await _stateProvinceService.GetStateProvinceByAbbreviationAsync(point.StateAbbreviation, country?.Id);

//                        var pickupPointModel = new CheckoutPickupPointModel
//                        {
//                            Id = point.Id,
//                            Name = point.Name,
//                            Description = point.Description,
//                            ProviderSystemName = point.ProviderSystemName,
//                            Address = point.Address,
//                            City = point.City,
//                            County = point.County,
//                            StateName = state != null ? await _localizationService.GetLocalizedAsync(state, x => x.Name, languageId) : string.Empty,
//                            CountryName = country != null ? await _localizationService.GetLocalizedAsync(country, x => x.Name, languageId) : string.Empty,
//                            ZipPostalCode = point.ZipPostalCode,
//                            Latitude = point.Latitude,
//                            Longitude = point.Longitude,
//                            OpeningHours = point.OpeningHours
//                        };

//                        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
//                        var amount = await _orderTotalCalculationService.IsFreeShippingAsync(cart) ? 0 : point.PickupFee;
//                        var currentCurrency = await _workContext.GetWorkingCurrencyAsync();

//                        if (amount > 0)
//                        {
//                            (amount, _) = await _taxService.GetShippingPriceAsync(amount, customer);
//                            amount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(amount, currentCurrency);
//                            pickupPointModel.PickupFee = await _priceFormatter.FormatShippingPriceAsync(amount, true);
//                        }

//                        //adjust rate
//                        var (shippingTotal, _) = await _orderTotalCalculationService.AdjustShippingRateAsync(point.PickupFee, cart, true);
//                        var (rateBase, _) = await _taxService.GetShippingPriceAsync(shippingTotal, customer);
//                        var rate = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(rateBase, currentCurrency);
//                        pickupPointModel.PickupFee = await _priceFormatter.FormatShippingPriceAsync(rate, true);

//                        return pickupPointModel;
//                    }).ToListAsync();
//                else
//                    foreach (var error in pickupPointsResponse.Errors)
//                        model.Warnings.Add(error);
//            }

//            //only available pickup points
//            var shippingProviders = await _shippingPluginManager.LoadActivePluginsAsync(customer, store.Id);
//            if (!shippingProviders.Any())
//            {
//                if (!pickupPointProviders.Any())
//                {
//                    model.Warnings.Add(await _localizationService.GetResourceAsync("Checkout.ShippingIsNotAllowed"));
//                    model.Warnings.Add(await _localizationService.GetResourceAsync("Checkout.PickupPoints.NotAvailable"));
//                }
//                model.PickupInStoreOnly = true;
//                model.PickupInStore = true;
//                return model;
//            }

//            return model;
//        }

//        #endregion
//    }
//}
