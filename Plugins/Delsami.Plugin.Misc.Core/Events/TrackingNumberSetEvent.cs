using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Events
{
    public class TrackingNumberSetEvent
    {
        public TrackingNumberSetEvent()
        {
        }

        public Shipment Shipment { get; set; }

        public bool SendSMS { get; set; }
    }
}
