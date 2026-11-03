using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Events
{
    public class PendingOrderCanceledEvent
    {
        public PendingOrderCanceledEvent(int orderId)
        {
            OrderId = orderId;
        }

        public int OrderId { get; set; }

    }
}