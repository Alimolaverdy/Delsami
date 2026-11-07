using Nop.Core.Domain.Shipping;
using Nop.Services.Shipping;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.DelsamiLogistic.Services
{
    public interface IDelsamiLogisticService
    {
        Task<IList<ShippingOption>> PrepareShippingOptionsAsync(GetShippingOptionRequest getShippingOptionRequest);
    }
}
