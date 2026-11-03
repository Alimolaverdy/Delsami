using Nop.Core.Domain.Catalog;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Domain;

namespace Delsami.Plugin.Misc.Core.Services
{
    public interface IReservedStockQuantityHistoryService
	{
		Task InsertAsync(Product product,
						 ReservedStockQuantity reservedStockQuantity,
						 string orderId,
						 int countAdjustment,
						 string message);
	}
}