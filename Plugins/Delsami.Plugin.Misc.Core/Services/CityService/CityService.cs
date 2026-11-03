using Nop.Core.Domain.Directory;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Domain;

namespace Delsami.Plugin.Misc.Core.Services.CityService
{
    public class CityService : ICityService
    {
        #region Fields

        private readonly IRepository<City> _cityRepo;
        private readonly IRepository<StateProvince> _stateProvinceRepo;

        #endregion

        #region Ctor

        public CityService(IRepository<City> cityRepo,
                           IRepository<StateProvince> stateProvinceRepo)
        {
            _cityRepo = cityRepo;
            _stateProvinceRepo = stateProvinceRepo;
        }

        #endregion

        #region Methods

        public async Task InsertAsync(City city)
        {
            if (city is null)
                throw new ArgumentNullException(nameof(city));

            await _cityRepo.InsertAsync(city);
        }

        public async Task<City> GetCityByCityAndStateProvinceNameAsync(string stateProvinceName, string cityName)
        {
            var stateProvince = await _stateProvinceRepo.Table.FirstOrDefaultAsync(w => stateProvinceName.Contains(w.Name));

            var cityQuery = _cityRepo.Table;

            if (stateProvince is not null)
                cityQuery.Where(w => w.StateProvinceId == stateProvince.Id);

            var city = await cityQuery.FirstOrDefaultAsync(w => w.Name == cityName);

            if (city is null)
                city = await _cityRepo.Table.FirstOrDefaultAsync(w => w.Name.Contains(cityName));

            if (city is null)
                city = await _cityRepo.Table.FirstOrDefaultAsync(w => cityName.Contains(w.Name));

            return city;
        }

        public async Task<List<City>> GetCitiesByStateProvinceIdAsync(int stateProvinceId)
        {
            var cities = await _cityRepo.Table.Where(w => w.StateProvinceId == stateProvinceId).OrderByDescending(o => o.Featured).ThenBy(o => o.Name).ToListAsync();

            return cities;
        }

        public async Task<City> GetCityByIdAsync(int cityId)
        {
            var city = await _cityRepo.Table.FirstOrDefaultAsync(w => w.Id == cityId);

            return city;
        }

        public async Task<City> GetCityByNameAsync(string cityName)
        {
            var city = await _cityRepo.Table.FirstOrDefaultAsync(w => w.Name == cityName);

            return city;
        }

        public async Task<List<City>> GetAllByProvinceId(int provinceId)
        {
            return await _cityRepo.Table.Where(w => w.StateProvinceId == provinceId).ToListAsync();
        }

        #endregion
    }
}