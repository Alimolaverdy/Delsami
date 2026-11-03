using Nop.Core;
using System;

namespace Delsami.Plugin.Misc.Core.Domain
{
	public class ReservedStockQuantityHistory : BaseEntity
	{
		public int ProductId { get; set; }

		public string OrderId { get; set; }

		public int ProductStockQuantity { get; set; }

		public int CountAdjustment { get; set; }

		public int Count { get; set; }

		public string Message { get; set; }

		public DateTime CreatedDateTime { get; set; }
	}
}