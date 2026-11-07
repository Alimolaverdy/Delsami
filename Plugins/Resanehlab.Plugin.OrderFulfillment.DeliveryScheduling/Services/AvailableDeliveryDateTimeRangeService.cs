using Nop.Core;
using Nop.Data;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Helpers;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services
{
    public class AvailableDeliveryDateTimeRangeService : IAvailableDeliveryDateTimeRangeService
    {
        private readonly IRepository<AvailableDeliveryDateTimeRange> _availableDeliveryDateTimeRangeRepo;
        private readonly IRepository<AvailableDeliveryTimeRange> _availableDeliveryTimeRangeRepo;
        private readonly INopDataProvider _dataProvider;

        public AvailableDeliveryDateTimeRangeService(
            IRepository<AvailableDeliveryDateTimeRange> availableDeliveryDateTimeRangeRepo,
            IRepository<AvailableDeliveryTimeRange> availableDeliveryTimeRangeRepo,
            INopDataProvider dataProvider
            )
        {
            _availableDeliveryDateTimeRangeRepo = availableDeliveryDateTimeRangeRepo;
            _availableDeliveryTimeRangeRepo = availableDeliveryTimeRangeRepo;
            _dataProvider = dataProvider;
        }

        public virtual async Task<AvailableDeliveryDateTimeRange> GetAvailableDeliveryDateTimeRangeByIdAsync(int id)
        {
            if (id == 0)
                return null;

            return await _availableDeliveryDateTimeRangeRepo.GetByIdAsync(id);
        }

        public virtual async Task<AvailableDeliveryDateTimeRange> GetAvailableDeliveryDateTimeRangeAsync(int dayOfWeek, int availableDeliveryTimeRangeId)
        {
            return await _availableDeliveryDateTimeRangeRepo.Table.Where(w => w.DayOfWeek == dayOfWeek && w.AvailableDeliveryTimeRangeId == availableDeliveryTimeRangeId).FirstOrDefaultAsync();
        }

        public virtual async Task<IPagedList<AvailableDeliveryDateTimeRange>> GetAvailableDeliveryDateTimeRangesAsync(DateTime? minEndDate = null, bool? isActive = null, int? storeId = null,
                                                        List<int> availableDeliveryTimeRangeIds = null, int pageIndex = 0, int pageSize = int.MaxValue, bool justExceptionsDateTimeRange = false)
        {
            var query = _availableDeliveryDateTimeRangeRepo.Table;

            if (minEndDate.HasValue)
                query = query.Where(w => w.EndDateOnUtc.HasValue && w.EndDateOnUtc >= minEndDate.Value);

            if (storeId.HasValue)
                query = from addtr in query
                        join adtr in _availableDeliveryTimeRangeRepo.Table on addtr.AvailableDeliveryTimeRangeId equals adtr.Id
                        where adtr.StoreId == storeId.Value && adtr.ExceptionDateId == null
                        select addtr;

            if (justExceptionsDateTimeRange)
            {

                query = from addtr in query
                        join adtr in _availableDeliveryTimeRangeRepo.Table on addtr.AvailableDeliveryTimeRangeId equals adtr.Id
                        where adtr.ExceptionDateId != null
                        select addtr;
            }
            else
            {
                query = from addtr in query
                        join adtr in _availableDeliveryTimeRangeRepo.Table on addtr.AvailableDeliveryTimeRangeId equals adtr.Id
                        where adtr.ExceptionDateId == null
                        select addtr;
            }

            if (isActive.HasValue)
                if (isActive.Value)
                    query = query.Where(w => !w.EndDateOnUtc.HasValue);
                else
                    query = query.Where(w => w.EndDateOnUtc.HasValue);

            if (availableDeliveryTimeRangeIds != null && availableDeliveryTimeRangeIds.Any())
                query = query.Where(t => availableDeliveryTimeRangeIds.Contains(t.AvailableDeliveryTimeRangeId));

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }


        public virtual async Task InsertAvailableDeliveryDateTimeRangeAsync(AvailableDeliveryDateTimeRange entity)
        {
            await _availableDeliveryDateTimeRangeRepo.InsertAsync(entity);
        }

        public virtual async Task UpdateAvailableDeliveryDateTimeRangeAsync(AvailableDeliveryDateTimeRange entity)
        {
            await _availableDeliveryDateTimeRangeRepo.UpdateAsync(entity);
        }

        public virtual async Task UpdateAvailableDeliveryDateTimeRangesAsync(IList<AvailableDeliveryDateTimeRange> entity)
        {
            await _availableDeliveryDateTimeRangeRepo.UpdateAsync(entity);
        }

        public virtual async Task<IList<AvailableDeliveryDateTimeModel>> LoadAvailableDeliveryDateTimesAsync(int shippingMethodId, int storeId, int languageId)
        {
            var pShippingMethodId = SqlParameterHelper.GetInt32Parameter("shippingMethodId", shippingMethodId);

            var pStoreId = SqlParameterHelper.GetInt32Parameter("storeId", storeId);

            var pLanguageId = SqlParameterHelper.GetInt32Parameter("languageId", languageId);


            //invoke stored procedure
            var result = await _dataProvider.QueryProcAsync<AvailableDeliveryDateTimeModel>(Constants.LoadAvailableDeliveryDateTimesStoredProcedureName, pShippingMethodId, pStoreId, pLanguageId);

            return result;
        }

        public virtual async Task<DateTime> GetLatestEndDateAsync(int storeId)
        {
            var query = from addtr in _availableDeliveryDateTimeRangeRepo.Table
                        join adtr in _availableDeliveryTimeRangeRepo.Table on addtr.AvailableDeliveryTimeRangeId equals adtr.Id
                        where adtr.StoreId == storeId && addtr.EndDateOnUtc.HasValue
                        orderby addtr.EndDateOnUtc descending
                        select addtr.EndDateOnUtc.Value;

            var result = await query.FirstOrDefaultAsync();

            return result;
        }

        public async Task<IList<AvailableDeliveryDateTimeRange>> GetByExceptionId(int exceptionId)
        {
            return await _availableDeliveryDateTimeRangeRepo.Table.Where(a => a.ExceptionDateId == exceptionId).ToListAsync();
        }

        public Task<List<AvailableDeliveryDateTimeRange>> GetForCustomExceptionsAsync(int storeId)
        {
            var query = _availableDeliveryDateTimeRangeRepo.Table;

                query = from addtr in query
                        join adtr in _availableDeliveryTimeRangeRepo.Table on addtr.AvailableDeliveryTimeRangeId equals adtr.Id
                        where adtr.StoreId == storeId && adtr.ExceptionDateId != null
                        select addtr;

            return query.ToListAsync(); 
        }
    }
}
