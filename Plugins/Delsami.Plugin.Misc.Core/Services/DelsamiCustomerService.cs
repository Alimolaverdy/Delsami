using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Services
{
    public class DelsamiCustomerService : IDelsamiCustomerService
    {
        #region Fields

        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly CoreSettings _coreSettings;
        private readonly ICustomerService _customerService;
        private readonly ICustomerAttributeValidationService _customerAttributeValidationService;


        #endregion

        #region Ctor

        public DelsamiCustomerService(ICustomerAttributeParser customerAttributeParser, ICustomerAttributeService customerAttributeService, CoreSettings coreSettings, ICustomerService customerService, ICustomerAttributeValidationService customerAttributeValidationService)
        {
            _customerAttributeParser = customerAttributeParser;
            _customerAttributeService = customerAttributeService;
            _coreSettings = coreSettings;
            _customerService = customerService;
            _customerAttributeValidationService = customerAttributeValidationService;
        }

        #endregion

        #region Methods

        public string GetNationalCode(string attributesXml)
        {
            var nationalCode = _customerAttributeParser.ParseValues(attributesXml, _coreSettings.NationalCodeAttributeId);

            if (nationalCode is null || !nationalCode.Any())
                return string.Empty;

            return nationalCode.FirstOrDefault(a => !string.IsNullOrWhiteSpace(a)).Trim() ?? string.Empty;
        }

        public async Task<bool> IsCheckoutCompletedAsync(Customer customer)
        {
            if (customer == null)
                return false;

            if (await _customerService.IsGuestAsync(customer))
                return true;

            List<CustomerAttribute> invalidStateCustomerAttributes = new();
            List<CustomerAttribute> nullOrEmptyCustomerAttributes = new();

            if (!string.IsNullOrWhiteSpace(customer.CustomCustomerAttributesXML))
            {
                invalidStateCustomerAttributes =
                    await _customerAttributeValidationService.GetInvalidCustomerAttributesAsync(customer.CustomCustomerAttributesXML);

                nullOrEmptyCustomerAttributes =
                    await _customerAttributeValidationService.GetNullOrEmptyRequiredCustomerAttributesAsync(customer.CustomCustomerAttributesXML);
            }

            invalidStateCustomerAttributes.AddRange(nullOrEmptyCustomerAttributes);

            if (invalidStateCustomerAttributes.Any() ||
                string.IsNullOrWhiteSpace(customer.FirstName) ||
                string.IsNullOrWhiteSpace(customer.LastName))
            {
                return false;
            }

            return true;
        }

        #endregion

    }

}
