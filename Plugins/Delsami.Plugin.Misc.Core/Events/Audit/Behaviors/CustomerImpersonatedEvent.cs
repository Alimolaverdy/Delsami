using DocumentFormat.OpenXml.Vml.Office;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Enums.Events;
using Delsami.Plugin.Misc.Core.Events.Audit.Base;
using Delsami.Plugin.Misc.Core.Models.Audit;

namespace Delsami.Plugin.Misc.Core.Events.Audit.Behaviors
{
    public class CustomerImpersonatedEvent : AuditBehaviorBaseEvent
    {
        public CustomerInfo CustomerInfo { get; set; }
        public CustomerImpersonatedEvent(Customer orginal, Customer impersonated, string descriptions = null)
        {
            BehaviorType = BehaviorEventType.Impersonate;
            Descriptions = descriptions;
            Entity = orginal;
            CustomerInfo = PrepareCustomerInfo(orginal, impersonated);
        }

        private CustomerInfo PrepareCustomerInfo(Customer orginal, Customer impersonated)
        {
            var orginalcustomer = new AuditCustomerModel()
            {
                CustomerId = orginal.Id,
                FirstName = orginal.FirstName,
                LastName = orginal.LastName,
                MobilePhoneNumber = orginal.Username
            };
            var impersonatedCustomer = new AuditCustomerModel()
            {
                CustomerId = impersonated.Id,
                FirstName = impersonated.FirstName,
                LastName = impersonated.LastName,
                MobilePhoneNumber = impersonated.Username
            };

            return new CustomerInfo()
            {
                ImpersonatedCustomer = impersonatedCustomer,
                OriginalCustomer = orginalcustomer,
            };
        }
    }
}