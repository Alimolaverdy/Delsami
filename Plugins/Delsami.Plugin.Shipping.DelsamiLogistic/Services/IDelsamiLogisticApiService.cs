using Delsami.Plugin.Shipping.DelsamiLogistic.Models;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.DelsamiLogistic.Services
{
    public interface IDelsamiLogisticApiService
    {
        Task<CloseCargoAndOrderBySellSiteResultModel> CloseCargoAndOrderBySellSiteAsync(int shipmentId);
    }
}
