using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Web.Models.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Domain;
using Delsami.Plugin.Misc.Core.Helpers;
using Nop.Web.Factories;

namespace Delsami.Plugin.Misc.Core.Services.ShippingMethodService
{
    public class CustomerAttributeValidationService : ICustomerAttributeValidationService
    {
        #region Fields

        private readonly IRepository<CustomerAttributeValidation> _customerAttributeValidatorRepo;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ICustomerModelFactory _customerModelFactory;
        private readonly IWorkContext _workContext;
        private readonly ICustomCustomerAttributesXMLService _customCustomerAttributesXMLService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Ctor

        public CustomerAttributeValidationService(IRepository<CustomerAttributeValidation> customerAttributeValidatorRepo,
            ICustomerAttributeParser customerAttributeParser,
            ICustomerAttributeService customerAttributeService,
            ICustomerModelFactory customerModelFactory,
            IWorkContext workContext,
            ICustomCustomerAttributesXMLService customCustomerAttributesXMLService,
            IHttpContextAccessor httpContextAccessor)
        {
            _customerAttributeValidatorRepo = customerAttributeValidatorRepo;
            _customerAttributeParser = customerAttributeParser;
            _customerAttributeService = customerAttributeService;
            _customerModelFactory = customerModelFactory;
            _workContext = workContext;
            _customCustomerAttributesXMLService = customCustomerAttributesXMLService;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Methods

        public async Task<IPagedList<CustomerAttributeValidation>> GetCustomerAttributeValidationsAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var customerCarsInfo = await _customerAttributeValidatorRepo.GetAllPagedAsync(query =>
            {
                return query.OrderBy(c => c.Id);

            }, pageIndex, pageSize);

            return customerCarsInfo;
        }

        public async Task InsertAsync(CustomerAttributeValidation customerAttributeValidation)
        {
            if (customerAttributeValidation == null)
                throw new ArgumentNullException(nameof(customerAttributeValidation));

            await _customerAttributeValidatorRepo.InsertAsync(customerAttributeValidation);
        }

        public async Task UpdateAsync(CustomerAttributeValidation customerAttributeValidation)
        {
            if (customerAttributeValidation == null)
                throw new ArgumentNullException(nameof(customerAttributeValidation));

            await _customerAttributeValidatorRepo.UpdateAsync(customerAttributeValidation);
        }

        public async Task<CustomerAttributeValidation> GetByIdAsync(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(id));

            return await _customerAttributeValidatorRepo.GetByIdAsync(id);
        }

        public async Task DeleteAsync(CustomerAttributeValidation customerAttributeValidation)
        {
            if (customerAttributeValidation == null)
                throw new ArgumentNullException(nameof(customerAttributeValidation));

            await _customerAttributeValidatorRepo.DeleteAsync(customerAttributeValidation);
        }

        public async Task<CustomerAttributeValidation> GetCustomerAttributeValidationByCustomerAttributeIdAsync(int customerAttributeId, int? editedEntityId = null)
        {
            var query = _customerAttributeValidatorRepo.Table.Where(w => w.CustomerAttributeId == customerAttributeId);

            if (editedEntityId.HasValue)
                query = query.Where(w => w.Id != editedEntityId);

            return await query.FirstOrDefaultAsync();

        }

        public async Task<List<CustomerAttribute>> GetInvalidCustomerAttributesAsync(Customer customer)
        {
            return await GetInvalidCustomerAttributesAsync(customer.CustomCustomerAttributesXML);
        }

        public async Task<List<CustomerAttribute>> GetInvalidCustomerAttributesAsync(string attributesXml)
        {
            List<CustomerAttribute> invalidStateCustomerAttributes = new();

            //ensure it's our attributes
            var attributes1 = await _customerAttributeParser.ParseCustomerAttributesAsync(attributesXml);

            //validate required customer attributes (whether they're chosen/selected/entered)
            var attributes2 = await _customerAttributeService.GetAllCustomerAttributesAsync();

            //get id of national code from CustomCustomerAttributes
            var nationalCodeAttributeId = attributes2.FirstOrDefault(a => a.AttributeControlType == AttributeControlType.TextBox
            && GetCustomerAttributeValidationByCustomerAttributeIdAsync(a.Id).Result.IsNationalCode == true).Id;

            var currentRoute = _httpContextAccessor.HttpContext.Request.Path.ToString()?.ToLower();
            int? excludedCustomerId = null;

            if (currentRoute.Contains("admin", StringComparison.InvariantCultureIgnoreCase) && currentRoute.Contains("edit", StringComparison.InvariantCultureIgnoreCase))
            {
                var customerIdStr = _httpContextAccessor.HttpContext.Request.Form["Id"];

                excludedCustomerId = int.Parse(customerIdStr);
            }
            else if (currentRoute.Contains("info", StringComparison.InvariantCultureIgnoreCase) || currentRoute.Contains("cart", StringComparison.InvariantCultureIgnoreCase) || currentRoute.Contains("onepagecheckout", StringComparison.InvariantCultureIgnoreCase))
            {
                excludedCustomerId = (await _workContext.GetCurrentCustomerAsync()).Id;
            }
            // get all national code except current user
            var allNationalCodes = await _customCustomerAttributesXMLService.GetAllCustomCustomersAttributesXMLAsync(nationalCodeAttributeId, excludedCustomerId);

            foreach (var a2 in attributes2.Where(w => w.AttributeControlType == AttributeControlType.TextBox))
            {
                var customerAttributeValidation = await GetCustomerAttributeValidationByCustomerAttributeIdAsync(a2.Id);

                if (customerAttributeValidation == null || !customerAttributeValidation.Active)
                    continue;

                var validValue = true;

                //selected customer attributes
                foreach (var a1 in attributes1)
                {
                    if (a1.Id != a2.Id)
                        continue;

                    var valuesStr = _customerAttributeParser.ParseValues(attributesXml, a1.Id);

                    var value = valuesStr.FirstOrDefault(str1 => !string.IsNullOrEmpty(str1.Trim()));

                    if (string.IsNullOrWhiteSpace(value))
                        continue;

                    if (customerAttributeValidation.IsNationalCode)
                    {
                        if (!ValidationHelper.IsValidNationalCode(value))
                            validValue = false;
                    }

                    if (allNationalCodes.Contains(value))
                    {
                        invalidStateCustomerAttributes.Add(new CustomerAttribute { Name = "NationalCodeDuplicate" });
                    }


                    else
                    {
                        var match = Regex.Match(value, $@"{customerAttributeValidation.RegexPattern}", RegexOptions.IgnoreCase);

                        if (!match.Success)
                            validValue = false;
                    }
                }

                if (validValue)
                    continue;

                invalidStateCustomerAttributes.Add(a2);
            }

            return invalidStateCustomerAttributes;
        }

        public async Task<List<CustomerAttribute>> GetNullOrEmptyRequiredCustomerAttributesAsync(string attributesXml)
        {
            var nullOrEmptyRequiredCustomerAttributes = new List<CustomerAttribute>();

            //ensure it's our attributes
            var attributes1 = await _customerAttributeParser.ParseCustomerAttributesAsync(attributesXml);

            //validate required customer attributes (whether they're chosen/selected/entered)
            var attributes2 = await _customerAttributeService.GetAllCustomerAttributesAsync();

            foreach (var a2 in attributes2)
            {
                if (!a2.IsRequired)
                    continue;

                var found = false;
                //selected customer attributes
                foreach (var a1 in attributes1)
                {
                    if (a1.Id != a2.Id)
                        continue;

                    var valuesStr = _customerAttributeParser.ParseValues(attributesXml, a1.Id);

                    found = valuesStr.Any(str1 => !string.IsNullOrEmpty(str1.Trim()));
                }

                if (found)
                    continue;

                //if not found
                nullOrEmptyRequiredCustomerAttributes.Add(a2);
            }

            return nullOrEmptyRequiredCustomerAttributes;
        }

        public async Task<bool> CurrentCustomerInfoCompletedAsync()
        {
            var model = new CustomerInfoModel();

            model = await _customerModelFactory.PrepareCustomerInfoModelAsync(model, await _workContext.GetCurrentCustomerAsync(), false);

            if (model.FirstNameEnabled && model.FirstNameRequired && string.IsNullOrWhiteSpace(model.FirstName))
                return false;

            if (model.LastNameEnabled && model.LastNameRequired && string.IsNullOrWhiteSpace(model.LastName))
                return false;

            if (model.DateOfBirthEnabled && model.DateOfBirthRequired && (!model.DateOfBirthYear.HasValue || !model.DateOfBirthMonth.HasValue || !model.DateOfBirthDay.HasValue))
                return false;

            if (model.CompanyEnabled && model.CompanyRequired && string.IsNullOrWhiteSpace(model.Company))
                return false;

            if (model.StreetAddressEnabled && model.StreetAddressRequired && string.IsNullOrWhiteSpace(model.StreetAddress))
                return false;

            if (model.StreetAddress2Enabled && model.StreetAddress2Required && string.IsNullOrWhiteSpace(model.StreetAddress2))
                return false;

            if (model.ZipPostalCodeEnabled && model.ZipPostalCodeRequired && string.IsNullOrWhiteSpace(model.ZipPostalCode))
                return false;

            if (model.CityEnabled && model.CityRequired && string.IsNullOrWhiteSpace(model.City))
                return false;

            if (model.CountyEnabled && model.CountyRequired && string.IsNullOrWhiteSpace(model.County))
                return false;

            if (model.CountryEnabled && model.CountryRequired && model.CountryId == 0)
                return false;

            if (model.StateProvinceEnabled && model.StateProvinceRequired && model.StateProvinceId == 0)
                return false;

            if (model.PhoneEnabled && model.PhoneRequired && string.IsNullOrWhiteSpace(model.Phone))
                return false;

            if (model.FaxEnabled && model.FaxRequired && string.IsNullOrWhiteSpace(model.Fax))
                return false;

            foreach (var customerAttribute in model.CustomerAttributes)
                if (customerAttribute.IsRequired && string.IsNullOrWhiteSpace(customerAttribute.DefaultValue) && !customerAttribute.Values.Any())
                    return false;

            return true;
        }

        #endregion
    }
}
