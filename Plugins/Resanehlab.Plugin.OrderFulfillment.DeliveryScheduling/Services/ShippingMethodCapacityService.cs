using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;
using Nop.Core;
using Nop.Data;
using NUglify.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services
{
    public class ShippingMethodCapacityService : IShippingMethodCapacityService
    {
        private readonly IRepository<ShippingMethodCapacity> _shippingMethodCapacityRepo;
        private readonly INopDataProvider _dataProvider;

        public ShippingMethodCapacityService(
            IRepository<ShippingMethodCapacity> shippingMethodCapacityRepo,
            INopDataProvider dataProvider)
        {
            _shippingMethodCapacityRepo = shippingMethodCapacityRepo;
            _dataProvider = dataProvider;
        }

        public virtual async Task InsertShippingMethodCapacityAsync(ShippingMethodCapacity shippingMethodCapacity)
        {
            if (shippingMethodCapacity == null)
                return;

            await _shippingMethodCapacityRepo.InsertAsync(shippingMethodCapacity);
        }

        public virtual async Task UpdateShippingMethodCapacityAsync(ShippingMethodCapacity shippingMethodCapacity)
        {
            if (shippingMethodCapacity == null)
                return;

            await _shippingMethodCapacityRepo.UpdateAsync(shippingMethodCapacity);
        }

        public virtual async Task<IPagedList<ShippingMethodCapacity>> GetShippingMethodCapacitiesAsync(int shippingMethodId = 0, int availableDeliveryDateTimeRangeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _shippingMethodCapacityRepo.Table;

            if (shippingMethodId > 0)
                query = query.Where(w => w.ShippingMethodId == shippingMethodId);
            if (availableDeliveryDateTimeRangeId > 0)
                query = query.Where(w => w.AvailableDeliveryDateTimeRangeId == availableDeliveryDateTimeRangeId);

            query = query.Where(w => !w.Deleted);

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        public virtual async Task DeleteShippingMethodCapacityAsync(ShippingMethodCapacity shippingMethodCapacity)
        {
            shippingMethodCapacity.Deleted = true;

            await _shippingMethodCapacityRepo.UpdateAsync(shippingMethodCapacity);
        }

        public virtual async Task DeleteByDateTimeIdsAsync(IList<int> Ids)
        {
            var list = await _shippingMethodCapacityRepo.Table.Where(a => Ids.Any(b => b == a.AvailableDeliveryDateTimeRangeId)).ToListAsync();
            list.ForEach(a => a.Deleted = true);
            await _shippingMethodCapacityRepo.UpdateAsync(list);
        }

        public virtual async Task<ShippingMethodCapacity> GetShippingMethodCapacityByIdAsync(int id)
        {
            return await _shippingMethodCapacityRepo.GetByIdAsync(id);
        }

        public async Task<IList<ShippingMethodCapacity>> GetByAvailableDeliveryDateTimeRangeIdsAsync(IList<int> ids, int? shippingMethodId = null)
        {
            var query = _shippingMethodCapacityRepo.Table;
            query=query.Where(a => ids.Contains(a.AvailableDeliveryDateTimeRangeId) && a.Deleted == false);

            if (shippingMethodId.HasValue)
                query=query.Where(a => a.ShippingMethodId == shippingMethodId);

            return await  query.ToListAsync();
        }

        public async Task DeleteAllNotCustomExceptionAsync(List<int> ids)
        {
            var list =await GetShippingMethodCapacitiesAsync();
            foreach (var item in list)
            {
                if(ids.Contains(item.AvailableDeliveryDateTimeRangeId)) continue;
                item.Deleted = true;
            }
            await _shippingMethodCapacityRepo.UpdateAsync(list);
        }
    }
}
