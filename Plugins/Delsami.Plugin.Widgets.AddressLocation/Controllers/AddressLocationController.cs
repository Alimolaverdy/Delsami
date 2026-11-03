using Delsami.Plugin.Misc.Core.Helpers;
using Delsami.Plugin.Misc.Core.Services.CityService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Delsami.Plugin.Widgets.AddressLocation.Domain;
using Delsami.Plugin.Widgets.AddressLocation.Models.Addresses;
using Delsami.Plugin.Widgets.AddressLocation.Models.NeshanModels;
using Delsami.Plugin.Widgets.AddressLocation.Services;
using Delsami.Plugin.Widgets.AddressLocation.Services.StateProvinceLocations;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Events;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Extensions;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Delsami.Plugin.Widgets.AddressLocation.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AddressLocationController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly AddressLocationSettings _addressLocationSettings;
        private readonly IStateProvinceLocationService _stateProvinceLocationService;
        private readonly ICityService _cityService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly AddressSettings _addressSettings;
        private readonly ICountryService _countryService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressService _addressService;
        private readonly IAddressCityService _addressCityService;
        private readonly IranBoundaryService _iranBoundaryService;
		private readonly IEventPublisher _eventPublisher;

		#endregion

		#region Ctor

		public AddressLocationController(ILocalizationService localizationService,
										 INotificationService notificationService,
										 AddressLocationSettings addressLocationSettings,
										 IStateProvinceLocationService stateProvinceLocationService,
										 ICityService cityService,
										 IranBoundaryService iranBoundaryService,
										 IWorkContext workContext,
										 ICustomerService customerService,
										 IAddressModelFactory addressModelFactory,
										 AddressSettings addressSettings,
										 ICountryService countryService,
										 IAddressAttributeParser addressAttributeParser,
										 IAddressService addressService,
										 IAddressCityService addressCityService,
										 IEventPublisher eventPublisher)
		{
			_localizationService = localizationService;
			_notificationService = notificationService;
			_addressLocationSettings = addressLocationSettings;
			_stateProvinceLocationService = stateProvinceLocationService;
			_cityService = cityService;
			_workContext = workContext;
			_customerService = customerService;
			_addressModelFactory = addressModelFactory;
			_addressSettings = addressSettings;
			_countryService = countryService;
			_addressAttributeParser = addressAttributeParser;
			_addressService = addressService;
			_addressCityService = addressCityService;
			_iranBoundaryService = iranBoundaryService;
			_eventPublisher = eventPublisher;
		}

		#endregion

		#region Methods

		[HttpGet]
        public async Task<IActionResult> Reverse(double lat, double lng)
        {

            if (!_iranBoundaryService.IsInside(lat, lng))
                return BadRequest(new
                {
                    message = await _localizationService.GetResourceAsync("Account.CustomerAddresses.latitudeValidationError")
                });

            var apiKey = _addressLocationSettings.NeshanServiceApiKey;

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromMilliseconds(_addressLocationSettings.NeshanServiceTimeoutAsMilliseconds);
            client.DefaultRequestHeaders.Add("Api-Key", apiKey);
            var url = $"https://api.neshan.org/v5/reverse?lat={lat.ToString(CultureInfo.InvariantCulture)}&lng={lng.ToString(CultureInfo.InvariantCulture)}";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();

            var neshanReverseResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<NeshanReverseResponseModel>(content);

            return Content(content, "application/json");
        }


        [HttpGet]
        public async Task<IActionResult> GetStateLocation(int stateProvinceId)
        {
            var location = await _stateProvinceLocationService.GetStateProvinceLocationByIdAsync(stateProvinceId);

            if (location == null)
                return NotFound();

            return Ok(new
            {
                latitude = location.Latitude,
                longitude = location.Longitude
            });
        }

        [HttpGet]
        public async Task<IActionResult> Geocode([FromQuery] string address)
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(3);
            client.DefaultRequestHeaders.Add("Api-Key", _addressLocationSettings.NeshanServiceApiKey);

            var encodedAddress = WebUtility.UrlEncode(address);
            var url = $"https://api.neshan.org/v6/geocoding?address={encodedAddress}";
            var json = await client.GetStringAsync(url);

            using var doc = JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty("location", out var loc))
                return NotFound();

            return Ok(new
            {
                x = loc.GetProperty("x").GetDouble(),
                y = loc.GetProperty("y").GetDouble()
            });
        }

        public virtual async Task<IActionResult> AddressAdd(bool? returnToCheckout)
        {
            if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
                return Challenge();

            var model = new DelsamiCustomerAddressEditModel();
            await _addressModelFactory.PrepareAddressModelAsync(model.Address,
                address: null,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: async () => await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id));

            ViewData["ReturnToCheckout"] = returnToCheckout;


            var currentCustomer = await _workContext.GetCurrentCustomerAsync();

            model.Address.FirstName = currentCustomer.FirstName;
            model.Address.LastName = currentCustomer.LastName;
            model.Address.PhoneNumber = currentCustomer.Username;

            return View("~/Plugins/Delsami.Widgets.AddressLocation/Views/AddressLocation/AddressAdd.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddressAdd(DelsamiCustomerAddressEditModel model, IFormCollection form, bool returnToCheckout)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(customer))
                return Challenge();

            if (model.Address.CityId != 0)
            {
                var city = await _cityService.GetCityByIdAsync(model.Address.CityId);

                model.Address.City = city?.Name;
            }
            //custom address attributes
            var customAttributes = await _addressAttributeParser.ParseCustomAddressAttributesAsync(form);
            var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                var address = model.Address.ToEntity();
                address.CustomAttributes = customAttributes;
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;


                await _addressService.InsertAddressAsync(address);

                await _customerService.InsertCustomerAddressAsync(customer, address);

                var addressCity = new AddressCity
                {
                    AddressId = address.Id,
                    CityId = model.Address.CityId
                };

                await _addressCityService.InsertAsync(addressCity);

				_notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Account.CustomerAddresses.Added"));

                if (!returnToCheckout)
                    return RedirectToAction("Addresses", "Customer");
                else
                    return RedirectToAction("OnePageCheckout", "DelsamiCheckout", new { t = GeneralHelper.GenerateUrlSafeString() });

            }

            //If we got this far, something failed, redisplay form
            await _addressModelFactory.PrepareAddressModelAsync(model.Address,
                address: null,
                excludeProperties: true,
                addressSettings: _addressSettings,
                loadCountries: async () => await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id),
                overrideAttributesXml: customAttributes);

            ViewData["ReturnToCheckout"] = returnToCheckout;
            ViewData["SaveMap"] = true;

            return View("~/Plugins/Delsami.Widgets.AddressLocation/Views/AddressLocation/AddressAdd.cshtml", model);
        }

        public virtual async Task<IActionResult> AddressEdit(int addressId)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(customer))
                return Challenge();

            //find address (ensure that it belongs to the current customer)
            var address = await _customerService.GetCustomerAddressAsync(customer.Id, addressId);
            if (address == null)
                //address is not found
                return RedirectToRoute("CustomerAddresses");

            var model = new DelsamiCustomerAddressEditModel();
            await _addressModelFactory.PrepareAddressModelAsync(model.Address,
                address: address,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: async () => await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id));

            var addressCity = await _addressCityService.GetAddressCityByAddressIdAsync(address.Id);

            if (addressCity != null)
            {
                model.Address.CityId = addressCity.CityId;
                var city = await _cityService.GetCityByIdAsync(addressCity.CityId);

                if (city is not null)
                    model.Address.City = city.Name;
            }
            return View("~/Plugins/Delsami.Widgets.AddressLocation/Views/AddressLocation/AddressEdit.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddressEdit(DelsamiCustomerAddressEditModel model, IFormCollection form)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(customer))
                return Challenge();

            if (model.Address.CityId != 0)
            {
                var city = await _cityService.GetCityByIdAsync(model.Address.CityId);

                model.Address.City = city?.Name;
            }

            //find address (ensure that it belongs to the current customer)
            var address = await _customerService.GetCustomerAddressAsync(customer.Id, model.Address.Id);
            if (address == null)
                //address is not found
                return RedirectToRoute("CustomerAddresses");

            //custom address attributes
            var customAttributes = await _addressAttributeParser.ParseCustomAddressAttributesAsync(form);
            var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                address.CustomAttributes = customAttributes;
                await _addressService.UpdateAddressAsync(address);


                var addressCity = await _addressCityService.GetAddressCityByAddressIdAsync(model.Address.Id);

                if (addressCity is null)
                {

                    addressCity = new AddressCity
                    {
                        AddressId = address.Id,
                        CityId = model.Address.CityId
                    };

                    await _addressCityService.InsertAsync(addressCity);
                }
                else
                {
                    addressCity.CityId = model.Address.CityId;

                    await _addressCityService.UpdateAsync(addressCity);
                }


				_notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Account.CustomerAddresses.Updated"));

                return RedirectToRoute("CustomerAddresses");
            }

            //If we got this far, something failed, redisplay form
            await _addressModelFactory.PrepareAddressModelAsync(model.Address,
                address: address,
                excludeProperties: true,
                addressSettings: _addressSettings,
                loadCountries: async () => await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id),
                overrideAttributesXml: customAttributes);

            return View("~/Plugins/Delsami.Widgets.AddressLocation/Views/AddressLocation/AddressEdit.cshtml", model);
        }

        public async Task<IActionResult> GetCitiesByStateProvinceId(int stateProvinceId)
        {
            var cities = await _cityService.GetCitiesByStateProvinceIdAsync(stateProvinceId);

            return Ok(cities);
        }

        private (double Lat, double Lng) GetLatitude(IFormCollection form)
        {

            var attrib1 = form.TryGetValue("address_attribute_1", out var value1)
                          ? value1.ToString()
                          : null;

            var attrib2 = form.TryGetValue("address_attribute_2", out var value2)
                          ? value2.ToString()
                          : null;

            if (attrib1 is null || attrib2 is null)
                return (0, 0);


            var lat = double.TryParse(attrib1, out double latValue) ? latValue : 0;
            var lng = double.TryParse(attrib2, out double lngValue) ? lngValue : 0;

            return (lat, lng);
        }
        #endregion
    }
}
