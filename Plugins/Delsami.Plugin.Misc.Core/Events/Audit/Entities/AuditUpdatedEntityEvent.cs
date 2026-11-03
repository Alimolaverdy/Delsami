using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Events.Audit.Base;

namespace Delsami.Plugin.Misc.Core.Events.Audit.Entities
{
    public class AuditUpdatedEntityEvent<T> : AuditEntityBaseEvent<T>
    {

        public AuditUpdatedEntityEvent(T newEntity, T oldEntity, dynamic parentEntity = default, string descriptions = null)
        {
            CurrentEntity = newEntity;
            OldEntity = oldEntity;
            Descriptions = descriptions;
            ParentEntity = parentEntity;

        }
    }
}