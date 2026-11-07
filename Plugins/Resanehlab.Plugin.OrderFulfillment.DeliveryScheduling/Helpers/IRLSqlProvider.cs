using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Models;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Helpers
{
    public interface IRLSqlProvider
    {
        Task CreateStoredProcedureAsync(RLBaseStoredProcedurePresenter storedProcedurePresenter);

        Task DropStoredProcedureAsync(string storedProcedureName);

        Task<bool> StoredProcedureExistsAsync(string storedProcedureName);
    }
}
