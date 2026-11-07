using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Common.Pdf;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Extensions;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Helpers;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services
{
    public class OrderShippingMethodCapacityMappingService : IOrderShippingMethodCapacityMappingService
    {
        #region Fields

        private readonly IRepository<OrderShippingMethodCapacityMapping> _orderShippingMethodCapacityMappingRepo;
        private readonly IEventPublisher _eventPublisher;
        private readonly INopDataProvider _dataProvider;
        private readonly AddressSettings _addressSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly INopFileProvider _fileProvider;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductService _productService;
        private readonly ISettingService _settingService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly PdfSettings _pdfSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly IGiftCardService _giftCardService;
        private readonly TaxSettings _taxSettings;
        private readonly IRewardPointService _rewardPointService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPaymentService _paymentService;
        private readonly IShippingMethodCapacityService _shippingMethodCapacityService;
        private readonly IAvailableDeliveryDateTimeRangeService _availableDeliveryDateTimeRangeService;
        private readonly IAvailableDeliveryTimeRangeService _availableDeliveryTimeRangeService;
        private readonly IHtmlFormatter _htmlFormatter;

        #endregion

        #region Ctor

        public OrderShippingMethodCapacityMappingService(IRepository<OrderShippingMethodCapacityMapping> orderShippingMethodCapacityMappingRepo,
            IEventPublisher eventPublisher,
            INopDataProvider dataProvider,
            AddressSettings addressSettings,
            CatalogSettings catalogSettings,
            IAddressAttributeFormatter addressAttributeFormatter,
            IAddressService addressService,
            ICountryService countryService,
            ICurrencyService currencyService,
            IDateTimeHelper dateTimeHelper,
            ILanguageService languageService,
            ILocalizationService localizationService,
            INopFileProvider fileProvider,
            IOrderService orderService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductService productService,
            ISettingService settingService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStoreService storeService,
            IVendorService vendorService,
            IWorkContext workContext,
            PdfSettings pdfSettings,
            VendorSettings vendorSettings,
            IGiftCardService giftCardService,
            TaxSettings taxSettings,
            IRewardPointService rewardPointService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IShippingMethodCapacityService shippingMethodCapacityService,
            IAvailableDeliveryDateTimeRangeService availableDeliveryDateTimeRangeService,
            IAvailableDeliveryTimeRangeService availableDeliveryTimeRangeService,
            IHtmlFormatter htmlFormatter
            )
        {
            _orderShippingMethodCapacityMappingRepo = orderShippingMethodCapacityMappingRepo;
            _eventPublisher = eventPublisher;
            _dataProvider = dataProvider;
            _addressSettings = addressSettings;
            _addressService = addressService;
            _catalogSettings = catalogSettings;
            _countryService = countryService;
            _addressAttributeFormatter = addressAttributeFormatter;
            _currencyService = currencyService;
            _dateTimeHelper = dateTimeHelper;
            _languageService = languageService;
            _localizationService = localizationService;
            _fileProvider = fileProvider;
            _orderService = orderService;
            _pictureService = pictureService;
            _priceFormatter = priceFormatter;
            _productService = productService;
            _settingService = settingService;
            _storeContext = storeContext;
            _stateProvinceService = stateProvinceService;
            _storeService = storeService;
            _vendorService = vendorService;
            _workContext = workContext;
            _pdfSettings = pdfSettings;
            _vendorSettings = vendorSettings;
            _giftCardService = giftCardService;
            _taxSettings = taxSettings;
            _rewardPointService = rewardPointService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _shippingMethodCapacityService = shippingMethodCapacityService;
            _availableDeliveryDateTimeRangeService = availableDeliveryDateTimeRangeService;
            _availableDeliveryTimeRangeService = availableDeliveryTimeRangeService;
            _htmlFormatter = htmlFormatter;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Write PDF invoice to the specified stream
        /// </summary>
        /// <param name="stream">Stream to save PDF</param>
        /// <param name="order">Order</param>
        /// <param name="language">Language; null to use a language used when placing an order</param>
        /// <param name="store">Store</param>
        /// <param name="vendor">Vendor to limit products; null to print all products. If specified, then totals won't be printed</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        protected virtual async Task PrintOrderToPdfAsync(Stream stream, Order order, Language language = null, Store store = null, Vendor vendor = null)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //store info
            store ??= await _storeContext.GetCurrentStoreAsync();

            var orderStore = order.StoreId == 0 || order.StoreId == store?.Id ?
                store : await _storeService.GetStoreByIdAsync(order.StoreId);

            //language info
            language ??= await _languageService.GetLanguageByIdAsync(order.CustomerLanguageId);

            if (language?.Published != true)
                language = await _workContext.GetWorkingLanguageAsync();

            //by default _pdfSettings contains settings for the current active store
            //and we need PdfSettings for the store which was used to place an order
            //so let's load it based on a store of the current order
            var pdfSettingsByStore = await _settingService.LoadSettingAsync<PdfSettings>(orderStore.Id);

            byte[] logo = null;
            var logoPicture = await _pictureService.GetPictureByIdAsync(pdfSettingsByStore.LogoPictureId);
            if (logoPicture != null)
            {
                var logoFilePath = await _pictureService.GetThumbLocalPathAsync(logoPicture, 0, false);

                if (logoPicture.MimeType == MimeTypes.ImageSvg)
                {
                    logo = await _pictureService.ConvertSvgToPngAsync(logoFilePath);
                }
                else
                {
                    logo = await _fileProvider.ReadAllBytesAsync(logoFilePath);
                }
            }

            var date = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc);

            //a vendor should have access only to products
            var orderItems = await _orderService.GetOrderItemsAsync(order.Id, vendorId: vendor?.Id ?? 0);

            var column1Lines = string.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn1) ?
                new List<string>()
                : pdfSettingsByStore.InvoiceFooterTextColumn1
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

            var column2Lines = string.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn2) ?
                new List<string>()
                : pdfSettingsByStore.InvoiceFooterTextColumn2
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

            var source = new InvoiceSource()
            {
                StoreUrl = orderStore.Url?.Trim('/'),
                Language = language,
                FontFamily = pdfSettingsByStore.FontFamily,
                OrderDateUser = date,
                LogoData = logo,
                OrderNumberText = order.CustomOrderNumber,
                PageSize = pdfSettingsByStore.LetterPageSizeEnabled ? PageSizes.Letter : PageSizes.A4,
                BillingAddress = await GetBillingAddressAsync(vendor, language, order),
                ShippingAddress = await GetShippingAddressAsync(language, order),
                Products = await GetOrderProductItemsAsync(order, orderItems, language),
                ShowSkuInProductList = _catalogSettings.ShowSkuOnProductDetailsPage,
                ShowVendorInProductList = _vendorSettings.ShowVendorOnOrderDetailsPage,
                CheckoutAttributes = vendor is null ? order.CheckoutAttributeDescription : string.Empty, //vendors cannot see checkout attributes
                Totals = vendor is null ? await GetTotalsAsync(language, order) : new(), //vendors cannot see totals
                OrderNotes = await GetOrderNotesAsync(pdfSettingsByStore, order, language),
                FooterTextColumn1 = column1Lines,
                FooterTextColumn2 = column2Lines
            };

            await using var pdfStream = new MemoryStream();
            new InvoiceDocument(source, _localizationService)
                .GeneratePdf(pdfStream);

            pdfStream.Position = 0;
            await pdfStream.CopyToAsync(stream);
        }

        /// <summary>
        /// Get product entries for document data source
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="orderItems">Collection of order items</param>
        /// <param name="language">Language</param>
        /// <returns>A task that contains collection of product entries</returns>
        protected virtual async Task<List<ProductItem>> GetOrderProductItemsAsync(Order order, IList<OrderItem> orderItems, Language language)
        {
            var vendors = _vendorSettings.ShowVendorOnOrderDetailsPage ? await _vendorService.GetVendorsByProductIdsAsync(orderItems.Select(item => item.ProductId).ToArray()) : new List<Vendor>();

            var result = new List<ProductItem>();

            foreach (var oi in orderItems)
            {
                var productItem = new ProductItem();
                var product = await _productService.GetProductByIdAsync(oi.ProductId);

                //product name
                productItem.Name = await _localizationService.GetLocalizedAsync(product, x => x.Name, language.Id);

                //attributes
                if (!string.IsNullOrEmpty(oi.AttributeDescription))
                {
                    var attributes = _htmlFormatter.ConvertHtmlToPlainText(oi.AttributeDescription, true, true);
                    productItem.ProductAttributes = attributes.Split('\n').ToList();
                }

                //SKU
                if (_catalogSettings.ShowSkuOnProductDetailsPage)
                    productItem.Sku = await _productService.FormatSkuAsync(product, oi.AttributesXml);

                //Vendor name
                if (_vendorSettings.ShowVendorOnOrderDetailsPage)
                    productItem.VendorName = vendors.FirstOrDefault(v => v.Id == product.VendorId)?.Name ?? string.Empty;

                //price
                string unitPrice;
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var unitPriceInclTaxInCustomerCurrency =
                        _currencyService.ConvertCurrency(oi.UnitPriceInclTax, order.CurrencyRate);
                    unitPrice = await _priceFormatter.FormatPriceAsync(unitPriceInclTaxInCustomerCurrency, true,
                        order.CustomerCurrencyCode, language.Id, true);
                }
                else
                {
                    //excluding tax
                    var unitPriceExclTaxInCustomerCurrency =
                        _currencyService.ConvertCurrency(oi.UnitPriceExclTax, order.CurrencyRate);
                    unitPrice = await _priceFormatter.FormatPriceAsync(unitPriceExclTaxInCustomerCurrency, true,
                        order.CustomerCurrencyCode, language.Id, false);
                }

                productItem.Price = unitPrice;

                //qty
                productItem.Quantity = oi.Quantity.ToString();

                //total
                string subTotal;
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var priceInclTaxInCustomerCurrency =
                        _currencyService.ConvertCurrency(oi.PriceInclTax, order.CurrencyRate);
                    subTotal = await _priceFormatter.FormatPriceAsync(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode,
                        language.Id, true);
                }
                else
                {
                    //excluding tax
                    var priceExclTaxInCustomerCurrency =
                        _currencyService.ConvertCurrency(oi.PriceExclTax, order.CurrencyRate);
                    subTotal = await _priceFormatter.FormatPriceAsync(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode,
                        language.Id, false);
                }

                productItem.Total = subTotal;

                result.Add(productItem);
            }

            return result;
        }

        /// <summary>
        /// Get billing address
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="lang">Language</param>
        /// <param name="order">Order</param>
        /// <returns>A task that contains address item</returns>
        protected virtual async Task<AddressItem> GetBillingAddressAsync(Vendor vendor, Language lang, Order order)
        {
            var addressResult = new AddressItem();

            var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            if (_addressSettings.CompanyEnabled && !string.IsNullOrEmpty(billingAddress.Company))
                addressResult.Company = billingAddress.Company;

            addressResult.Name = $"{billingAddress.FirstName} {billingAddress.LastName}";

            if (_addressSettings.PhoneEnabled)
                addressResult.Phone = billingAddress.PhoneNumber;

            if (_addressSettings.FaxEnabled && !string.IsNullOrEmpty(billingAddress.FaxNumber))
                addressResult.Fax = billingAddress.FaxNumber;

            if (_addressSettings.StreetAddressEnabled)
                addressResult.Address = billingAddress.Address1;

            if (_addressSettings.StreetAddress2Enabled && !string.IsNullOrEmpty(billingAddress.Address2))
                addressResult.Address2 = billingAddress.Address2;

            if (_addressSettings.CityEnabled || _addressSettings.StateProvinceEnabled ||
                _addressSettings.CountyEnabled || _addressSettings.ZipPostalCodeEnabled)
            {
                addressResult.AddressLine =
                    $"{billingAddress.City}, " +
                    $"{(!string.IsNullOrEmpty(billingAddress.County) ? $"{billingAddress.County}, " : string.Empty)}" +
                    $"{(await _stateProvinceService.GetStateProvinceByAddressAsync(billingAddress) is StateProvince stateProvince ? await _localizationService.GetLocalizedAsync(stateProvince, x => x.Name, lang.Id) : string.Empty)} " +
                    $"{billingAddress.ZipPostalCode}";
            }

            if (_addressSettings.CountryEnabled && await _countryService.GetCountryByAddressAsync(billingAddress) is Country country)
                addressResult.Country = await _localizationService.GetLocalizedAsync(country, x => x.Name, lang.Id);

            //VAT number
            if (!string.IsNullOrEmpty(order.VatNumber))
                addressResult.VATNumber = order.VatNumber;

            //custom attributes
            var customBillingAddressAttributes = await _addressAttributeFormatter
                .FormatAttributesAsync(billingAddress.CustomAttributes, "<br />");

            if (!string.IsNullOrEmpty(customBillingAddressAttributes))
            {
                var text = _htmlFormatter.ConvertHtmlToPlainText(customBillingAddressAttributes, true, true);
                addressResult.AddressAttributes = text.Split('\n').ToList();
            }

            //vendors payment details
            if (vendor is null)
            {
                //payment method
                var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(order.PaymentMethodSystemName);
                var paymentMethodStr = paymentMethod != null
                    ? await _localizationService.GetLocalizedFriendlyNameAsync(paymentMethod, lang.Id)
                    : order.PaymentMethodSystemName;
                if (!string.IsNullOrEmpty(paymentMethodStr))
                {
                    addressResult.PaymentMethod = paymentMethodStr;
                }

                //custom values
                var customValues = _paymentService.DeserializeCustomValues(order);
                if (customValues != null)
                    addressResult.CustomValues = customValues;
            }

            return addressResult;
        }

        /// <summary>
        /// Get shipping address
        /// </summary>
        /// <param name="lang">Language</param>
        /// <param name="order">Order</param>
        /// <returns>A task that contains address item</returns>
        protected virtual async Task<AddressItem> GetShippingAddressAsync(Language lang, Order order)
        {
            var addressResult = new AddressItem();

            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                if (!order.PickupInStore)
                {
                    if (order.ShippingAddressId == null || await _addressService.GetAddressByIdAsync(order.ShippingAddressId.Value) is not Address shippingAddress)
                        throw new NopException($"Shipping is required, but address is not available. Order ID = {order.Id}");

                    if (!string.IsNullOrEmpty(shippingAddress.Company))
                        addressResult.Company = shippingAddress.Company;

                    addressResult.Name = $"{shippingAddress.FirstName} {shippingAddress.LastName}";

                    if (_addressSettings.PhoneEnabled)
                        addressResult.Phone = shippingAddress.PhoneNumber;

                    if (_addressSettings.FaxEnabled && !string.IsNullOrEmpty(shippingAddress.FaxNumber))
                        addressResult.Fax = shippingAddress.FaxNumber;

                    if (_addressSettings.StreetAddressEnabled)
                        addressResult.Address = shippingAddress.Address1;

                    if (_addressSettings.StreetAddress2Enabled && !string.IsNullOrEmpty(shippingAddress.Address2))
                        addressResult.Address2 = shippingAddress.Address2;

                    if (_addressSettings.CityEnabled || _addressSettings.StateProvinceEnabled ||
                        _addressSettings.CountyEnabled || _addressSettings.ZipPostalCodeEnabled)
                    {
                        addressResult.AddressLine = $"{shippingAddress.City}, " +
                            $"{(!string.IsNullOrEmpty(shippingAddress.County) ? $"{shippingAddress.County}, " : string.Empty)}" +
                            $"{(await _stateProvinceService.GetStateProvinceByAddressAsync(shippingAddress) is StateProvince stateProvince ? await _localizationService.GetLocalizedAsync(stateProvince, x => x.Name, lang.Id) : string.Empty)} " +
                            $"{shippingAddress.ZipPostalCode}";
                    }

                    if (_addressSettings.CountryEnabled && await _countryService.GetCountryByAddressAsync(shippingAddress) is Country country)
                    {
                        addressResult.Country = await _localizationService.GetLocalizedAsync(country, x => x.Name, lang.Id);
                    }

                    //custom attributes
                    var customShippingAddressAttributes = await _addressAttributeFormatter
                        .FormatAttributesAsync(shippingAddress.CustomAttributes, "<br />");
                    if (!string.IsNullOrEmpty(customShippingAddressAttributes))
                    {
                        var text = _htmlFormatter.ConvertHtmlToPlainText(customShippingAddressAttributes, true, true);
                        addressResult.AddressAttributes = text.Split('\n').ToList();
                    }
                }
                else if (order.PickupAddressId.HasValue && await _addressService.GetAddressByIdAsync(order.PickupAddressId.Value) is Address pickupAddress)
                {
                    if (!string.IsNullOrEmpty(pickupAddress.Address1))
                        addressResult.Address = pickupAddress.Address1;

                    if (_addressSettings.CityEnabled || _addressSettings.StateProvinceEnabled ||
                        _addressSettings.CountyEnabled || _addressSettings.ZipPostalCodeEnabled)
                    {
                        addressResult.AddressLine = $"{pickupAddress.City}, " +
                            $"{(!string.IsNullOrEmpty(pickupAddress.County) ? $"{pickupAddress.County}, " : string.Empty)}" +
                            $"{(await _stateProvinceService.GetStateProvinceByAddressAsync(pickupAddress) is StateProvince stateProvince ? await _localizationService.GetLocalizedAsync(stateProvince, x => x.Name, lang.Id) : string.Empty)} " +
                            $"{pickupAddress.ZipPostalCode}";
                    }

                    if (await _countryService.GetCountryByAddressAsync(pickupAddress) is Country country)
                        addressResult.Country = await _localizationService.GetLocalizedAsync(country, x => x.Name, lang.Id);
                }

                addressResult.ShippingMethod = order.ShippingMethod;

                var orderShippingMethodCapacityMapping = await GetOrderShippingMethodCapacityMappingByOrderIdAsync(order.Id);
                if (orderShippingMethodCapacityMapping != null)
                {
                    var customValues = new Dictionary<string, object>();

                    customValues.Add(await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryPdf.DeliveryInformation.DeliveryDate", lang.Id), await orderShippingMethodCapacityMapping.DeliveryDateOnUtc.FormatDateTimeAsync());

                    var shippingMethodCapacity = await _shippingMethodCapacityService.GetShippingMethodCapacityByIdAsync(orderShippingMethodCapacityMapping.ShippingMethodCapacityId);
                    if (shippingMethodCapacity != null)
                    {
                        var availableDeliveryDateTimeRange = await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangeByIdAsync(shippingMethodCapacity.AvailableDeliveryDateTimeRangeId);
                        if (availableDeliveryDateTimeRange != null)
                        {
                            var availableDeliveryTimeRange = await _availableDeliveryTimeRangeService.GetAvailableDeliveryTimeRangeByIdAsync(availableDeliveryDateTimeRange.AvailableDeliveryTimeRangeId);
                            if (availableDeliveryTimeRange != null)
                            {
                                customValues.Add(await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryPdf.DeliveryInformation.DeliveryTime", lang.Id), await _localizationService.GetLocalizedAsync(availableDeliveryTimeRange, d => d.Time));
                            }
                        }
                    }

                    addressResult.CustomValues = customValues;
                }
            }

            return addressResult;
        }

        /// <summary>
        /// Get invoice totals
        /// </summary>
        /// <param name="lang">Language</param>
        /// <param name="order">Order</param>
        /// <returns>A task that contains invoice totals</returns>
        protected virtual async Task<InvoiceTotals> GetTotalsAsync(Language lang, Order order)
        {
            var result = new InvoiceTotals();
            var languageId = lang.Id;

            //order subtotal
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax &&
                !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
            {
                //including tax
                var orderSubtotalInclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
                result.SubTotal = await _priceFormatter.FormatPriceAsync(orderSubtotalInclTaxInCustomerCurrency, true,
                    order.CustomerCurrencyCode, languageId, true);
            }
            else
            {
                //excluding tax
                var orderSubtotalExclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(order.OrderSubtotalExclTax, order.CurrencyRate);
                result.SubTotal = await _priceFormatter.FormatPriceAsync(orderSubtotalExclTaxInCustomerCurrency, true,
                    order.CustomerCurrencyCode, languageId, false);
            }

            //discount (applied to order subtotal)
            if (order.OrderSubTotalDiscountExclTax > decimal.Zero)
            {
                //order subtotal
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax &&
                    !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
                {
                    //including tax
                    var orderSubTotalDiscountInclTaxInCustomerCurrency =
                        _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
                    result.Discount = await _priceFormatter.FormatPriceAsync(
                        -orderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                }
                else
                {
                    //excluding tax
                    var orderSubTotalDiscountExclTaxInCustomerCurrency =
                        _currencyService.ConvertCurrency(order.OrderSubTotalDiscountExclTax, order.CurrencyRate);
                    result.Discount = await _priceFormatter.FormatPriceAsync(
                        -orderSubTotalDiscountExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                }
            }

            //shipping
            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var orderShippingInclTaxInCustomerCurrency =
                        _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
                    result.Shipping = await _priceFormatter.FormatShippingPriceAsync(
                        orderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                }
                else
                {
                    //excluding tax
                    var orderShippingExclTaxInCustomerCurrency =
                        _currencyService.ConvertCurrency(order.OrderShippingExclTax, order.CurrencyRate);
                    result.Shipping = await _priceFormatter.FormatShippingPriceAsync(
                        orderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                }
            }

            //payment fee
            if (order.PaymentMethodAdditionalFeeExclTax > decimal.Zero)
            {
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var paymentMethodAdditionalFeeInclTaxInCustomerCurrency =
                        _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
                    result.PaymentMethodAdditionalFee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(
                        paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                }
                else
                {
                    //excluding tax
                    var paymentMethodAdditionalFeeExclTaxInCustomerCurrency =
                        _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
                    result.PaymentMethodAdditionalFee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(
                        paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                }
            }

            //tax
            var taxStr = string.Empty;
            var taxRates = new SortedDictionary<decimal, decimal>();
            bool displayTax;
            var displayTaxRates = true;
            if (_taxSettings.HideTaxInOrderSummary && order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                displayTax = false;
            }
            else
            {
                if (order.OrderTax == 0 && _taxSettings.HideZeroTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    taxRates = _orderService.ParseTaxRates(order, order.TaxRates);

                    displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                    displayTax = !displayTaxRates;

                    var orderTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTax, order.CurrencyRate);
                    taxStr = await _priceFormatter.FormatPriceAsync(orderTaxInCustomerCurrency, true, order.CustomerCurrencyCode,
                        false, languageId);
                }
            }

            if (displayTax)
            {
                result.Tax = taxStr;
            }

            if (displayTaxRates)
            {
                foreach (var item in taxRates)
                {
                    var taxRate = string.Format(await _localizationService.GetResourceAsync("Pdf.TaxRate", languageId),
                        _priceFormatter.FormatTaxRate(item.Key));
                    var taxValue = await _priceFormatter.FormatPriceAsync(
                        _currencyService.ConvertCurrency(item.Value, order.CurrencyRate), true, order.CustomerCurrencyCode,
                        false, languageId);

                    result.TaxRates.Add($"{taxRate} {taxValue}");
                }
            }

            //discount (applied to order total)
            if (order.OrderDiscount > decimal.Zero)
            {
                var orderDiscountInCustomerCurrency =
                    _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
                result.Discount = await _priceFormatter.FormatPriceAsync(-orderDiscountInCustomerCurrency,
                    true, order.CustomerCurrencyCode, false, languageId);
            }

            //gift cards
            foreach (var gcuh in await _giftCardService.GetGiftCardUsageHistoryAsync(order))
            {
                var gcTitle = string.Format(await _localizationService.GetResourceAsync("Pdf.GiftCardInfo", languageId),
                    (await _giftCardService.GetGiftCardByIdAsync(gcuh.GiftCardId))?.GiftCardCouponCode);
                var gcAmountStr = await _priceFormatter.FormatPriceAsync(
                    -_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate), true,
                    order.CustomerCurrencyCode, false, languageId);

                result.GiftCards.Add($"{gcTitle} {gcAmountStr}");
            }

            //reward points
            if (order.RedeemedRewardPointsEntryId.HasValue && await _rewardPointService.GetRewardPointsHistoryEntryByIdAsync(order.RedeemedRewardPointsEntryId.Value) is RewardPointsHistory redeemedRewardPointsEntry)
            {
                var rpTitle = string.Format(await _localizationService.GetResourceAsync("Pdf.RewardPoints", languageId),
                    -redeemedRewardPointsEntry.Points);
                var rpAmount = await _priceFormatter.FormatPriceAsync(
                    -_currencyService.ConvertCurrency(redeemedRewardPointsEntry.UsedAmount, order.CurrencyRate),
                    true, order.CustomerCurrencyCode, false, languageId);

                result.RewardPoints = $"{rpTitle} {rpAmount}";
            }

            //order total
            var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
            var orderTotalStr = await _priceFormatter.FormatPriceAsync(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, languageId);
            result.OrderTotal = $"{await _localizationService.GetResourceAsync("Pdf.OrderTotal", languageId)} {orderTotalStr}";

            return result;
        }

        /// <summary>
        /// Get order notes
        /// </summary>
        /// <param name="pdfSettingsByStore">PDF settings</param>
        /// <param name="order">Order</param>
        /// <param name="lang">Language</param>
        /// <returns>A task that contains collection of date/note pairs</returns>
        protected virtual async Task<List<(string, string)>> GetOrderNotesAsync(PdfSettings pdfSettingsByStore, Order order, Language lang)
        {
            var notesResult = new List<(string, string)>();

            if (!pdfSettingsByStore.RenderOrderNotes)
                return notesResult;

            var orderNotes = (await _orderService.GetOrderNotesByOrderIdAsync(order.Id, true))
                .OrderByDescending(on => on.CreatedOnUtc)
                .ToList();

            if (!orderNotes.Any())
                return notesResult;

            foreach (var orderNote in orderNotes)
            {
                var createdOn = (await _dateTimeHelper.ConvertToUserTimeAsync(orderNote.CreatedOnUtc, DateTimeKind.Utc)).ToString();
                var note = _htmlFormatter.ConvertHtmlToPlainText(_orderService.FormatOrderNoteText(orderNote), true, true);

                notesResult.Add((createdOn, note));

                //should we display a link to downloadable files here?
                //I think, no. Anyway, PDFs are printable documents and links (files) are useful here
            }

            return notesResult;
        }

        #endregion

        #region Methods

        public virtual async Task InsertOrderShippingMethodCapacityMappingAsync(OrderShippingMethodCapacityMapping orderShippingMethodCapacityMapping)
        {
            if (orderShippingMethodCapacityMapping == null)
                throw new ArgumentNullException("orderShippingMethodCapacityMapping");

            await _orderShippingMethodCapacityMappingRepo.InsertAsync(orderShippingMethodCapacityMapping);

            //event notification
            await _eventPublisher.EntityInsertedAsync(orderShippingMethodCapacityMapping);
        }

        public virtual async Task<OrderShippingMethodCapacityMapping> GetOrderShippingMethodCapacityMappingByOrderIdAsync(int orderId)
        {
            return await _orderShippingMethodCapacityMappingRepo.Table.Where(w => w.OrderId == orderId).FirstOrDefaultAsync();
        }

        public virtual async Task<IPagedList<OrderShippingMethodCapacityMapping>> GetOrderShippingMethodCapacitiesAsync(int shippingMethodCapacityId = 0, int shippingMethodId = 0, DateTime? fromDate = null, DateTime? toDate = null, int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var pShippingMethodCapacityId = SqlParameterHelper.GetInt32Parameter("shippingMethodCapacityId", shippingMethodCapacityId);

            var pShippingMethodId = SqlParameterHelper.GetInt32Parameter("shippingMethodId", shippingMethodId);

            var pFromDate = SqlParameterHelper.GetDateTimeParameter("fromDate", fromDate);

            var pToDate = SqlParameterHelper.GetDateTimeParameter("toDate", toDate);

            var pStoreId = SqlParameterHelper.GetInt32Parameter("storeId", storeId);

            //some databases don't support int.MaxValue
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;

            var pPageSize = SqlParameterHelper.GetInt32Parameter("PageSize", pageSize);

            var pPageIndex = SqlParameterHelper.GetInt32Parameter("PageIndex", pageIndex);

            var pTotalRecords = SqlParameterHelper.GetOutputInt32Parameter("totalRecords");

            var orderShippingMethodCapacityMappings = (await _dataProvider.QueryProcAsync<OrderShippingMethodCapacityMapping>(Constants.GetOrderShippingMethodCapacitiesStoredProcedureName, pShippingMethodCapacityId, pShippingMethodId, pFromDate, pToDate, pStoreId, pPageSize, pPageIndex, pTotalRecords)).ToList();

            var totalRecords = pTotalRecords.Value != DBNull.Value ? Convert.ToInt32(pTotalRecords.Value) : 0;
            return new PagedList<OrderShippingMethodCapacityMapping>(orderShippingMethodCapacityMappings, pageIndex, pageSize, totalRecords);
        }

        public virtual async Task<IList<GetPlacedOrdersCountModel>> GetPlacedOrdersCountAsync(int storeId)
        {
            var pStoreId = SqlParameterHelper.GetInt32Parameter("storeId", storeId);

            //invoke stored procedure
            var result = await _dataProvider.QueryProcAsync<GetPlacedOrdersCountModel>(Constants.GetPlacedOrdersCountStoredProcedureName, pStoreId);

            return result;
        }

        public virtual async Task PrintOrdersToPdfAsync(Stream stream, IList<Order> orders, Language language = null, Vendor vendor = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (orders == null)
                throw new ArgumentNullException(nameof(orders));

            var currentStore = await _storeContext.GetCurrentStoreAsync();

            using var archive = new ZipArchive(stream, ZipArchiveMode.Create, true);

            foreach (var order in orders)
            {
                var entryName = string.Format("{0} {1}", await _localizationService.GetResourceAsync("Pdf.Order"), order.CustomOrderNumber);

                await using var fileStreamInZip = archive.CreateEntry($"{entryName}.pdf").Open();
                await using var pdfStream = new MemoryStream();
                await PrintOrderToPdfAsync(pdfStream, order, language, currentStore, vendor);
                pdfStream.Position = 0;
                await pdfStream.CopyToAsync(fileStreamInZip);
            }
        }

        #endregion
    }
}
