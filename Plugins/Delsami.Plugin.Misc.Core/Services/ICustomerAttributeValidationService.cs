using Nop.Core.Domain.Customers;
using Nop.Core;
using Nop.Web.Areas.Admin.Validators.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Domain;

namespace Delsami.Plugin.Misc.Core.Services
{
    public interface ICustomerAttributeValidationService
    {
        Task<IPagedList<CustomerAttributeValidation>> GetCustomerAttributeValidationsAsync(
                                                                        int pageIndex = 0,
                                                                        int pageSize = int.MaxValue);

        Task InsertAsync(CustomerAttributeValidation customerAttributeValidation);

        Task UpdateAsync(CustomerAttributeValidation customerAttributeValidation);

        Task<CustomerAttributeValidation> GetByIdAsync(int id);

        Task<List<CustomerAttribute>> GetInvalidCustomerAttributesAsync(Customer customer);

        Task<List<CustomerAttribute>> GetInvalidCustomerAttributesAsync(string attributesXml);

        Task<List<CustomerAttribute>> GetNullOrEmptyRequiredCustomerAttributesAsync(string attributesXml);
        Task DeleteAsync(CustomerAttributeValidation customerAttributeValidation);

        Task<CustomerAttributeValidation> GetCustomerAttributeValidationByCustomerAttributeIdAsync(int customerAttributeId, int? editedEntityId = null);

        Task<bool> CurrentCustomerInfoCompletedAsync();

    }
}