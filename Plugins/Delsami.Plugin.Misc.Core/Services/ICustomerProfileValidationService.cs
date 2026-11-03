using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;

namespace Delsami.Plugin.Misc.Core.Services
{
    public interface ICustomerProfileValidationService
    {
        Task<bool> IsCheckoutCompletedAsync(Customer customer);

    }
}
