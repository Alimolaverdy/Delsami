using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;
using Nop.Core;
using Nop.Data;
using Nop.Web.Framework.Models.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services
{
    public class AvailableDeliveryTimeRangeService : IAvailableDeliveryTimeRangeService
    {
        private readonly IRepository<AvailableDeliveryTimeRange> _availableDeliveryTimeRangeRepo;
        private readonly IRepository<AvailableDeliveryDateTimeRange> _availableDeliveryDateTimeRepo;

        public AvailableDeliveryTimeRangeService(IRepository<AvailableDeliveryTimeRange> availableDeliveryTimeRangeRepo, IRepository<AvailableDeliveryDateTimeRange> availableDeliveryDateTimeRepo)
        {
            _availableDeliveryTimeRangeRepo = availableDeliveryTimeRangeRepo;
            _availableDeliveryDateTimeRepo = availableDeliveryDateTimeRepo;
        }

        public virtual async Task<IPagedList<AvailableDeliveryTimeRange>> GetAvailableDeliveryTimeRangesAsync(int? storeId = null, int pageIndex = 0, int pageSize = int.MaxValue, bool justExceptionsTimeRange = false)
        {
            var query = _availableDeliveryTimeRangeRepo.Table;

            if (storeId.HasValue)
                query = query.Where(w => w.StoreId == storeId);

            if (justExceptionsTimeRange)
                query = query.Where(w => w.ExceptionDateId != null);
            else
                query = query.Where(w => w.ExceptionDateId == null);

            query = query.Where(w => !w.Deleted);

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        public virtual async Task InsertAvailableDeliveryTimeRangeAsync(AvailableDeliveryTimeRange entity)
        {
            if (entity == null)
                return;

            await _availableDeliveryTimeRangeRepo.InsertAsync(entity);
        }

        public virtual async Task DeleteAvailableDeliveryTimeRangeAsync(AvailableDeliveryTimeRange entity)
        {
            if (entity == null)
                return;

            entity.Deleted = true;

            await _availableDeliveryTimeRangeRepo.UpdateAsync(entity);
        }

        public virtual async Task UpdateAvailableDeliveryTimeRangeAsync(AvailableDeliveryTimeRange entity)
        {
            if (entity == null)
                return;

            await _availableDeliveryTimeRangeRepo.UpdateAsync(entity);
        }

        public virtual async Task<AvailableDeliveryTimeRange> GetAvailableDeliveryTimeRangeByIdAsync(int id)
        {
            if (id == 0)
                return null;

            return await _availableDeliveryTimeRangeRepo.GetByIdAsync(id);
        }

        public async Task<IPagedList<AvailableDeliveryTimeRangeModel>> GetTimeRangeByExceptionDateIdAsync(int exceptionDateId, int storeId)
        {
            var dateTimeQuery = await _availableDeliveryTimeRangeRepo.GetAllAsync(query =>
            {
                query = query.Where(a => a.ExceptionDateId == exceptionDateId && a.StoreId == storeId && a.Deleted == false);
                return query;
            });

            var models = dateTimeQuery.Select(a => new AvailableDeliveryTimeRangeModel()
            {
                Id = a.Id,
                EndHour = a.EndHour,
                StartHour = a.StartHour,
                ShippingMethodIds = a.ShippingMethodIds.Split("-").Select(int.Parse).ToList(),
                Time = a.Time,
                StoreId = a.StoreId,
                DisplayOrder = a.DisplayOrder,
            }).ToList();
            return new PagedList<AvailableDeliveryTimeRangeModel>(models,0,1000);
        }

        public async Task<AvailableDeliveryTimeRange> GetActiveByIdAsync(int id, int storeId)
        {
            return await _availableDeliveryTimeRangeRepo.Table.FirstOrDefaultAsync(a => a.Id == id && a.StoreId == storeId && a.Deleted == false);
        }

        public async Task<List<AvailableDeliveryTimeRange>> GetAllByIdsAsync(List<int> ids)
        {
            return await _availableDeliveryTimeRangeRepo.Table.Where(a=> ids.Contains(a.Id)).ToListAsync();
        }
    }
}
