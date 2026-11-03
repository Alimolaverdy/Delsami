using Nop.Core.Domain.Customers;
using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Delsami.Plugin.Misc.Core.Services
{
    public class CustomCustomerAttributesXMLService : ICustomCustomerAttributesXMLService
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IWorkContext _workContext;
        public CustomCustomerAttributesXMLService(IRepository<Customer> customer, IWorkContext workContext)
        {
            _customerRepository = customer;
            _workContext = workContext;
        }

        public async Task<List<string>> GetAllCustomCustomersAttributesXMLAsync(int id, int? excludedCustomerId = null)
        {

            var query = from c in _customerRepository.Table
                        where !string.IsNullOrEmpty(c.CustomCustomerAttributesXML)
                        select c;

            if (excludedCustomerId.HasValue)
                query = from c in query where c.Id != excludedCustomerId.Value select c;

            var customers = await query.ToListAsync();

            List<string> values = new List<string>();

            foreach (var customer in customers)
            {
                var xmlDoc = XElement.Parse(customer.CustomCustomerAttributesXML);

                var valuesForId = xmlDoc.Descendants("CustomerAttribute")
                                        .Where(attr => (int)attr.Attribute("ID") == id)
                                        .Select(attr => (string)attr.Descendants("Value").FirstOrDefault())
                                        .ToList();

                values.AddRange(valuesForId);
            }

            return values;
        }
    }
}
