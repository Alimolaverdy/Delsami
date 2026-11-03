using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Services.ShippingMethodService;

namespace Delsami.Plugin.Misc.Core.Services.ShippingMethod
{
    public class ShippingMethodService : IShippingMethodService
    {

        private readonly IRepository<Nop.Core.Domain.Shipping.ShippingMethod> _shippingMethodepository;

        public ShippingMethodService(IRepository<Nop.Core.Domain.Shipping.ShippingMethod> shippingMethodepository)
        {
            _shippingMethodepository = shippingMethodepository;
        }

        public async Task<Nop.Core.Domain.Shipping.ShippingMethod> GetShippingMethodByNameAsync(string name)
        {
            {

                if (string.IsNullOrWhiteSpace(name))
                    return null;

                name = name.Trim();

                var shippingMethods = await _shippingMethodepository.Table
                    .Where(sm => sm.Name != null)
                    .ToListAsync();

                var shippingMethod = shippingMethods
                    .FirstOrDefault(sm => sm.Name.Trim().Equals(name, StringComparison.InvariantCultureIgnoreCase));

                return shippingMethod;
            }
        }
    }
}
