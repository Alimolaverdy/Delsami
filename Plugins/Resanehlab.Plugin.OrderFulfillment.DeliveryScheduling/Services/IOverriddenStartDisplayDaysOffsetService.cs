using Nop.Core;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services
{
    public interface IOverriddenStartDisplayDaysOffsetService
    {
        Task<OverriddenStartDisplayDaysOffset> GetOverriddenStartDisplayDaysOffsetByIdAsync(int overriddenStartDisplayDaysOffsetId);

        Task<IPagedList<OverriddenStartDisplayDaysOffset>> GetAllOverriddenStartDisplayDaysOffsetsAsync(int[] categoryIds = null, int pageIndex = 0, int pageSize = int.MaxValue);

        Task InsertOverriddenStartDisplayDaysOffsetAsync(OverriddenStartDisplayDaysOffset overriddenStartDisplayDaysOffset);

        Task UpdateOverriddenStartDisplayDaysOffsetAsync(OverriddenStartDisplayDaysOffset overriddenStartDisplayDaysOffset);

        Task DeleteOverriddenStartDisplayDaysOffsetAsync(OverriddenStartDisplayDaysOffset overriddenStartDisplayDaysOffset);
    }
}
