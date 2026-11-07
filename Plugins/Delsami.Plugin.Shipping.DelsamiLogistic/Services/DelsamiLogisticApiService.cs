using Delsami.Plugin.Shipping.DelsamiLogistic.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.DelsamiLogistic.Services
{
    public class DelsamiLogisticApiService : IDelsamiLogisticApiService
    {
        #region Fields

        private readonly DelsamiLogisticSettings _settings;

        #endregion

        #region Ctor

        public DelsamiLogisticApiService(DelsamiLogisticSettings settings)
        {
            _settings = settings;
        }

        #endregion

        #region  Methods

        public async Task<CloseCargoAndOrderBySellSiteResultModel> CloseCargoAndOrderBySellSiteAsync(int shipmentId)
        {
            try
            {
                using (var clinet = new HttpClient())
                {
                    var payload = new { shipmentId, token = _settings.ApiToken };

                    var url = $"{_settings.ApiBaseUrl}/couriers/closecargoandorderbysellsite";

                    var response = await clinet.PostAsJsonAsync(url, payload);

                    string result = await response.Content.ReadAsStringAsync();

                    return Newtonsoft.Json.JsonConvert.DeserializeObject<CloseCargoAndOrderBySellSiteResultModel>(result);
                }
            }
            catch
            {
                return new CloseCargoAndOrderBySellSiteResultModel
                {
                    message = "خطای اتصال به لجستیک",
                    value = false,
                    statusCode = 0
                };
            }
        }

        #endregion
    }
}
