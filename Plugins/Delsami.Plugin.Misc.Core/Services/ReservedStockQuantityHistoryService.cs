using Nop.Core.Domain.Catalog;
using Nop.Data;
using System;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Domain;

namespace Delsami.Plugin.Misc.Core.Services
{
	public class ReservedStockQuantityHistoryService : IReservedStockQuantityHistoryService
	{
		#region Fields

		private readonly IRepository<ReservedStockQuantityHistory> _reservedStockQuantityHistoryRepo;

		#endregion

		#region Ctor

		public ReservedStockQuantityHistoryService(IRepository<ReservedStockQuantityHistory> reservedStockQuantityHistoryRepo)
		{
			_reservedStockQuantityHistoryRepo = reservedStockQuantityHistoryRepo;
		}

		#endregion

		#region Methods

		public async Task InsertAsync(Product product,
									  ReservedStockQuantity reservedStockQuantity,
									  string orderId,
									  int countAdjustment,
									  string message)
		{
			if (product is null)
				throw new ArgumentNullException(nameof(product));

			if (reservedStockQuantity is null)
				throw new ArgumentNullException(nameof(reservedStockQuantity));

			var reservedQuantityHistory = new ReservedStockQuantityHistory()
			{
				CountAdjustment = countAdjustment,
				Count = reservedStockQuantity.Count,
				Message = message,
				OrderId = orderId,
				CreatedDateTime = DateTime.UtcNow,
				ProductId = product.Id,
				ProductStockQuantity = product.StockQuantity
			};

			await _reservedStockQuantityHistoryRepo.InsertAsync(reservedQuantityHistory);
		}

		#endregion
	}
}