using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Events
{
    public class DeliveryCodeGeneratedEvent
    {
        public DeliveryCodeGeneratedEvent(int shipmentId, int deliveryCode)
        {
            ShipmentId = shipmentId;
            DeliveryCode = deliveryCode;
        }

        public int ShipmentId { get; set; }

        public int DeliveryCode { get; set; }
    }
}
