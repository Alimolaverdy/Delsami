using Nop.Core;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services
{
    public interface IShippingMethodCapacityService
    {
        Task<ShippingMethodCapacity> GetShippingMethodCapacityByIdAsync(int id);
        Task<IList<ShippingMethodCapacity>> GetByAvailableDeliveryDateTimeRangeIdsAsync(IList<int> ids,int? shippingMethodId=null);

        Task InsertShippingMethodCapacityAsync(ShippingMethodCapacity shippingMethodCapacity);

        Task UpdateShippingMethodCapacityAsync(ShippingMethodCapacity shippingMethodCapacity);

        Task<IPagedList<ShippingMethodCapacity>> GetShippingMethodCapacitiesAsync(int shippingMethodId = 0, int availableDeliveryDateTimeRangeId = 0, int pageIndex = 0, int pageSize = int.MaxValue);

        Task DeleteShippingMethodCapacityAsync(ShippingMethodCapacity shippingMethodCapacity);
        Task DeleteByDateTimeIdsAsync(IList<int> Ids);

        Task DeleteAllNotCustomExceptionAsync(List<int> ids);
    }
}
