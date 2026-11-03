using System.Threading.Tasks;
using Delsami.Plugin.Widgets.AddressLocation.Domain;

namespace Delsami.Plugin.Widgets.AddressLocation.Services.StateProvinceLocations
{
    public interface IStateProvinceLocationService
    {
        Task<StateProvinceLocation> GetStateProvinceLocationByIdAsync(int stateProvinceId);
    }
}
