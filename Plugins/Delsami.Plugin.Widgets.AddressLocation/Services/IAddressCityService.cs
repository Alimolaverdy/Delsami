using Delsami.Plugin.Widgets.AddressLocation.Domain;
using System.Threading.Tasks;

namespace Delsami.Plugin.Widgets.AddressLocation.Services
{
    public interface IAddressCityService
    {
        Task InsertAsync(AddressCity addressCity);

        Task<AddressCity> GetAddressCityByAddressIdAsync(int addressId);

        Task UpdateAsync(AddressCity addressCity);
    }
}
