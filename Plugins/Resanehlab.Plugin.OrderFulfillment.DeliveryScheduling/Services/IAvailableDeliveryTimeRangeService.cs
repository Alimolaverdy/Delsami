using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;
using Nop.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services
{
    public interface IAvailableDeliveryTimeRangeService
    {
        Task<AvailableDeliveryTimeRange> GetAvailableDeliveryTimeRangeByIdAsync(int id);

        Task<AvailableDeliveryTimeRange> GetActiveByIdAsync(int id, int storeId);

        Task<List<AvailableDeliveryTimeRange>> GetAllByIdsAsync(List<int> ids);

        Task<IPagedList<AvailableDeliveryTimeRange>> GetAvailableDeliveryTimeRangesAsync(int? storeId = null, int pageIndex = 0, int pageSize = int.MaxValue, bool justExceptionsTimeRange = false);

        Task InsertAvailableDeliveryTimeRangeAsync(AvailableDeliveryTimeRange entity);

        Task DeleteAvailableDeliveryTimeRangeAsync(AvailableDeliveryTimeRange entity);

        Task UpdateAvailableDeliveryTimeRangeAsync(AvailableDeliveryTimeRange entity);

        Task<IPagedList<AvailableDeliveryTimeRangeModel>> GetTimeRangeByExceptionDateIdAsync(int exceptionDateId, int storeId);

    }
}
