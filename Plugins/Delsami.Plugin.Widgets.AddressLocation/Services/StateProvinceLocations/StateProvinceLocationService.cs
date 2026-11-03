using Delsami.Plugin.Widgets.AddressLocation.Domain;
using Nop.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Widgets.AddressLocation.Services.StateProvinceLocations
{
    public class StateProvinceLocationService : IStateProvinceLocationService
    {
        #region Fields
        private readonly IRepository<StateProvinceLocation> _stateProvinceLocationRepository;
        #endregion

        #region Ctor
        public StateProvinceLocationService(IRepository<StateProvinceLocation> stateProvinceLocationRepository)
        {
            _stateProvinceLocationRepository = stateProvinceLocationRepository;
        }
        #endregion

        #region MEthods
        public async Task<StateProvinceLocation> GetStateProvinceLocationByIdAsync(int stateProvinceId)
        {
            return await _stateProvinceLocationRepository.Table.FirstOrDefaultAsync(i => i.StateProvinceId == stateProvinceId);
        }
        #endregion
    }
}
