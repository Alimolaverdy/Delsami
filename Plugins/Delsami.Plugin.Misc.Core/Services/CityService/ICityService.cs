using System.Collections.Generic;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Domain;

namespace Delsami.Plugin.Misc.Core.Services.CityService
{
    public interface ICityService
    {
        Task<City> GetCityByCityAndStateProvinceNameAsync(string stateProvinceName, string cityName);
        Task InsertAsync(City city);
        Task<List<City>> GetCitiesByStateProvinceIdAsync(int stateProvinceId);
        Task<City> GetCityByIdAsync(int cityId);
        Task<City> GetCityByNameAsync(string cityName);
        Task<List<City>> GetAllByProvinceId(int provinceId);

    }
}
