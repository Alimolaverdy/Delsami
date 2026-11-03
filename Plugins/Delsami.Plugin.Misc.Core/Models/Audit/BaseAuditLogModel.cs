using System;

namespace Delsami.Plugin.Misc.Core.Models.Audit
{
    public class BaseAuditLogModel
    {
        public Guid Id { get; set; }

        public CustomerInfo Customer { get; set; }

        public DateTime OccurredAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Descriptions { get; set; }

    }


    public class CustomerInfo
    {
        public AuditCustomerModel OriginalCustomer { get; set; }
        public AuditCustomerModel ImpersonatedCustomer { get; set; }
    }
}
