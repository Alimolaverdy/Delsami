using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Events
{
    public class OrderProcessingStartEvent
    {
        public OrderProcessingStartEvent(Order order)
        {
            Order = order;
        }

        public Order Order { get; set; }
    }
}