using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Orders;
using System;
using System.Linq;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Domain;

namespace Delsami.Plugin.Misc.Core.Services
{
	public class ReservedStockQuantityService : IReservedStockQuantityService
	{
		#region Fields

		private readonly IRepository<ReservedStockQuantity> _reservedStockQuantityRepo;
		private readonly IProductService _productService;
		private readonly IOrderService _orderService;
		private readonly IReservedStockQuantityHistoryService _reservedStockQuantityHistory;
		private readonly ILocalizationService _localizationService;

		#endregion

		#region Ctor

		public ReservedStockQuantityService(IRepository<ReservedStockQuantity> reservedStockQuantityRepo,
											IProductService productService,
											IOrderService orderService,
											IReservedStockQuantityHistoryService reservedStockQuantityHistory,
											ILocalizationService localizationService)
		{
			_reservedStockQuantityRepo = reservedStockQuantityRepo;
			_productService = productService;
			_orderService = orderService;
			_reservedStockQuantityHistory = reservedStockQuantityHistory;
			_localizationService = localizationService;
		}

		#endregion

		#region Methods

		public async Task<int> GetReservedStockQuantityCountAsync(int productId)
		{

			var reservedStockQuantity = await GetReservedStockQuantityAsync(productId);

			if (reservedStockQuantity is null)
				return 0;

			return reservedStockQuantity.Count;
		}

		public async Task<ReservedStockQuantity> GetReservedStockQuantityAsync(int productId)
		{
			return await _reservedStockQuantityRepo.Table.FirstOrDefaultAsync(f => f.ProductId == productId);
		}

		public async Task InsertAsync(ReservedStockQuantity reservedStockQuantity)
		{
			if (reservedStockQuantity == null)
				throw new ArgumentNullException(nameof(reservedStockQuantity));

			await _reservedStockQuantityRepo.InsertAsync(reservedStockQuantity);
		}

		public async Task UpdateAsync(ReservedStockQuantity reservedStockQuantity)
		{
			if (reservedStockQuantity == null)
				throw new ArgumentNullException(nameof(reservedStockQuantity));

			reservedStockQuantity.LastModifiedDateTime = DateTime.UtcNow;

			await _reservedStockQuantityRepo.UpdateAsync(reservedStockQuantity);
		}

		public async Task AdjustReservedStockQuantityAsync(Product product, string orderId, int quantity, string message)
		{

			var reservedStockQuantity = await GetReservedStockQuantityAsync(product.Id);

			if (reservedStockQuantity is null)
			{
				reservedStockQuantity = new ReservedStockQuantity
				{
					ProductId = product.Id,
					LastModifiedDateTime = DateTime.UtcNow
				};

				await InsertAsync(reservedStockQuantity);
			}

			reservedStockQuantity.Count += quantity;

			await UpdateAsync(reservedStockQuantity);
			await _reservedStockQuantityHistory.InsertAsync(product, reservedStockQuantity, orderId, quantity, message);
		}

		public async Task ReverseReservedStockQuantityAsync(Order order, string message)
		{
			var orderItems = await _orderService.GetOrderItemsAsync(order.Id);

			foreach (var orderItem in orderItems)
			{
				await AdjustReservedStockQuantityAsync(orderItem.ProductId, order.Id, -orderItem.Quantity, message);
			}
		}

		#endregion

		#region Utilities

		private async Task AdjustReservedStockQuantityAsync(int productId, int orderId, int quantity, string message)
		{
			var product = await _productService.GetProductByIdAsync(productId);

			if (product is null)
				throw new ArgumentNullException(nameof(product));

			await AdjustReservedStockQuantityAsync(product, orderId.ToString(), quantity, message);
		}

		#endregion
	}
}