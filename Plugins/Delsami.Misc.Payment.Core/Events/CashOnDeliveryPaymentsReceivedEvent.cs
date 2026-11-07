using System.Collections.Generic;

namespace Delsami.Plugin.Payment.Core.Events
{
    public class CashOnDeliveryPaymentsReceivedEvent
    {
        public CashOnDeliveryPaymentsReceivedEvent(List<Domain.Payment> payments)
        {
            Payments = payments;
        }

        public List<Domain.Payment> Payments { get; set; }

    }
}
