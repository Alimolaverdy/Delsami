using Delsami.Plugin.Widgets.AddressLocation.Domain;
using Nop.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Widgets.AddressLocation.Services
{
    public class AddressCityService : IAddressCityService
    {
        private readonly IRepository<AddressCity> _addressCityRepo;

        public AddressCityService(IRepository<AddressCity> addressCityRepo)
        {
            _addressCityRepo = addressCityRepo;
        }

        public async Task InsertAsync(AddressCity addressCity)
        {
            if (addressCity is null)
                throw new ArgumentNullException(nameof(addressCity));

            await _addressCityRepo.InsertAsync(addressCity);
        }

        public async Task<AddressCity> GetAddressCityByAddressIdAsync(int addressId)
        {
            var addressCity = await _addressCityRepo.Table.FirstOrDefaultAsync(f => f.AddressId == addressId);

            return addressCity;
        }

        public async Task UpdateAsync(AddressCity addressCity)
        {
            if (addressCity is null)
                throw new ArgumentNullException(nameof(addressCity));

            await _addressCityRepo.UpdateAsync(addressCity);
        }
    }
}
