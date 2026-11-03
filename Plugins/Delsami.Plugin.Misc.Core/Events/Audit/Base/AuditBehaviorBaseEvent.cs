using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Enums.Events;

namespace Delsami.Plugin.Misc.Core.Events.Audit.Base
{
    public class AuditBehaviorBaseEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public BehaviorEventType BehaviorType { get; set; }

        public dynamic Entity { get; set; }

        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

        public string Descriptions { get; init; }

    }
}
