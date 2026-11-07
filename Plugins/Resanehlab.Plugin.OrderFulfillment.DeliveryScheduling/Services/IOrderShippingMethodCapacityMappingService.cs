using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services
{
    public interface IOrderShippingMethodCapacityMappingService
    {
        Task InsertOrderShippingMethodCapacityMappingAsync(OrderShippingMethodCapacityMapping orderShippingMethodCapacityMapping);

        Task<OrderShippingMethodCapacityMapping> GetOrderShippingMethodCapacityMappingByOrderIdAsync(int orderId);

        Task<IPagedList<OrderShippingMethodCapacityMapping>> GetOrderShippingMethodCapacitiesAsync(int shippingMethodCapacityId = 0, int shippingMethodId = 0, DateTime? fromDate = null, DateTime? toDate = null, int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Write ZIP archive with invoices to the specified stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="orders">Orders</param>
        /// <param name="language">Language; null to use a language used when placing an order</param>
        /// <param name="vendor">Vendor to limit products; null to print all products. If specified, then totals won't be printed</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrintOrdersToPdfAsync(Stream stream, IList<Order> orders, Language language = null, Vendor vendor = null);

        Task<IList<GetPlacedOrdersCountModel>> GetPlacedOrdersCountAsync(int storeId);
    }
}
