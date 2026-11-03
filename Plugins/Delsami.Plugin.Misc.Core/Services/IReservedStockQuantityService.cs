using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Domain;

namespace Delsami.Plugin.Misc.Core.Services
{
	public interface IReservedStockQuantityService
	{
		Task<int> GetReservedStockQuantityCountAsync(int productId);

		Task<ReservedStockQuantity> GetReservedStockQuantityAsync(int productId);

		Task InsertAsync(ReservedStockQuantity reservedStockQuantity);

		Task UpdateAsync(ReservedStockQuantity reservedStockQuantity);

		Task AdjustReservedStockQuantityAsync(Product product, string orderId, int quantity, string message);

		Task ReverseReservedStockQuantityAsync(Order order, string message);

	}
}