using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Events.Audit.Base;

namespace Delsami.Plugin.Misc.Core.Events.Audit.Entities
{
    public class AuditDeletedEntityEvent<T> : AuditEntityBaseEvent<T>
    {

        public AuditDeletedEntityEvent(T Entity, dynamic parentEntity = default, string descriptions = null)
        {
            CurrentEntity = Entity;
            Descriptions = descriptions;
            ParentEntity = parentEntity;

        }
    }
}