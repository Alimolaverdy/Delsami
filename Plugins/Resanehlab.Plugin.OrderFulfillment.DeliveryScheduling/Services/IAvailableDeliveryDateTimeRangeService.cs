using Nop.Core;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services
{
    public interface IAvailableDeliveryDateTimeRangeService
    {
        Task<AvailableDeliveryDateTimeRange> GetAvailableDeliveryDateTimeRangeByIdAsync(int id);

        Task<AvailableDeliveryDateTimeRange> GetAvailableDeliveryDateTimeRangeAsync(int dayOfWeek, int availableDeliveryTimeRangeId);

        Task InsertAvailableDeliveryDateTimeRangeAsync(AvailableDeliveryDateTimeRange entity);

        Task UpdateAvailableDeliveryDateTimeRangeAsync(AvailableDeliveryDateTimeRange entity);

        Task UpdateAvailableDeliveryDateTimeRangesAsync(IList<AvailableDeliveryDateTimeRange> entity);

        Task<IPagedList<AvailableDeliveryDateTimeRange>> GetAvailableDeliveryDateTimeRangesAsync(DateTime? minEndDate = null, bool? isActive = null, int? storeId = null, List<int> availableDeliveryTimeRangeIds = null, int pageIndex = 0, int pageSize = int.MaxValue, bool justExceptionsDateTimeRange = false);
        
        Task<List<AvailableDeliveryDateTimeRange>> GetForCustomExceptionsAsync(int storeId);

        Task<IList<AvailableDeliveryDateTimeModel>> LoadAvailableDeliveryDateTimesAsync(int shippingMethodId, int storeId, int languageId);

        Task<DateTime> GetLatestEndDateAsync(int storeId);

        Task<IList<AvailableDeliveryDateTimeRange>> GetByExceptionId(int exceptionId);
    }
}
