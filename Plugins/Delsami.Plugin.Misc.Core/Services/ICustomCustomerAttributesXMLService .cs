using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Services
{
    public interface ICustomCustomerAttributesXMLService
    {
        Task<List<string>> GetAllCustomCustomersAttributesXMLAsync(int Id, int? excludedCustomerId = null);
    }
}
