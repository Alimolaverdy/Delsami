using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Events.Audit.Base
{
    public class AuditEntityBaseEvent<T>
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime OccurredAt { get; set; } = DateTime.Now;

        public string Descriptions { get; init; }

        public T CurrentEntity { get; init; }

        public T OldEntity { get; init; }

        public dynamic ParentEntity { get; init; }

    }
}
