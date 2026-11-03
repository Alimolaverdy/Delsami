using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Events.Audit.Base;

namespace Delsami.Plugin.Misc.Core.Models.Audit
{
    public class AuditBehaviorLogModel : BaseAuditLogModel
    {
        public string BehaviorType { get; set; }

        public dynamic Entity { get; set; }

        public static AuditBehaviorLogModel BuildLogModel(AuditBehaviorBaseEvent eventMsg, CustomerInfo customer)
        {
            return new AuditBehaviorLogModel
            {
                Id = eventMsg.Id,
                Descriptions = eventMsg.Descriptions,
                OccurredAt = eventMsg.OccurredAt.ToLocalTime(),
                Customer = customer,
                Entity = eventMsg.Entity,
                BehaviorType = eventMsg.BehaviorType.ToString(),
            };
        }
    }
}
