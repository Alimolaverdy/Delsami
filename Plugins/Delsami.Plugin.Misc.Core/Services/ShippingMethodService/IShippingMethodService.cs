
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Services.ShippingMethodService
{
    public interface IShippingMethodService
    {
        Task<Nop.Core.Domain.Shipping.ShippingMethod> GetShippingMethodByNameAsync(string name);

    }
}
