using Nop.Data;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Helpers
{
    public class MSSqlProvider : IRLSqlProvider
    {
        private readonly INopDataProvider _dataProvider;

        public MSSqlProvider(INopDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task<bool> StoredProcedureExistsAsync(string storedProcedureName)
        {
            return (await _dataProvider.QueryAsync<IntegerQueryResultModel>(
                string.Format("SELECT 1 AS [Result] FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}]') AND type in (N'P', N'PC')",
                storedProcedureName))).FirstOrDefault() != null;
        }

        public async Task CreateStoredProcedureAsync(RLBaseStoredProcedurePresenter storedProcedurePresenter)
        {
            if (!await StoredProcedureExistsAsync(storedProcedurePresenter.StoredProcedureName))
                await _dataProvider.ExecuteNonQueryAsync(storedProcedurePresenter.MsSqlBody);
        }

        public async Task DropStoredProcedureAsync(string storedProcedureName)
        {
            if (await StoredProcedureExistsAsync(storedProcedureName))
            {
                var dbScript = "DROP PROCEDURE [" + storedProcedureName + "]";
                await _dataProvider.ExecuteNonQueryAsync(dbScript);
            }
        }
    }
}
