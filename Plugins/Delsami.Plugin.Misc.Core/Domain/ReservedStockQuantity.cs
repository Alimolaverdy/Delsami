using Nop.Core;
using System;

namespace Delsami.Plugin.Misc.Core.Domain
{
	public class ReservedStockQuantity : BaseEntity
	{
		public int ProductId { get; set; }

		public int Count { get; set; }

		public DateTime LastModifiedDateTime { get; set; }

	}
}