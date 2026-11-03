using Newtonsoft.Json;
using Nop.Core;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Enums.Events;
using Delsami.Plugin.Misc.Core.Events.Audit.Base;
using Delsami.Plugin.Misc.Core.Helpers;

namespace Delsami.Plugin.Misc.Core.Models.Audit
{
    public class AuditEntityLogModel : BaseAuditLogModel
    {
        public string EntityId { get; set; }

        public ExpandoObject ChangedValues { get; set; }

        public dynamic CurrentEntity { get; set; }

        public dynamic ParentEntity { get; set; }

        public EventTypes EventType { get; set; }


        public static AuditEntityLogModel BuildUpdateLogModel<T>(AuditEntityBaseEvent<T> eventMsg, T oldEntity, dynamic parentEntity, CustomerInfo customer) where T : BaseEntity
        {
            var values = FastDiff<T>.GetDiffJson(eventMsg.CurrentEntity, oldEntity);

            if (values == "{}" || string.IsNullOrEmpty(values)) return null;

            return new AuditEntityLogModel
            {
                Id = eventMsg.Id,
                EventType = EventTypes.Update,
                ChangedValues = JsonConvert.DeserializeObject<ExpandoObject>(values),
                CurrentEntity = eventMsg.CurrentEntity,
                Descriptions = eventMsg.Descriptions,
                EntityId = eventMsg.CurrentEntity.Id.ToString(),
                OccurredAt = eventMsg.OccurredAt,
                Customer = customer,
                ParentEntity = parentEntity,

            };
        }
        public static AuditEntityLogModel BuildCreateLogModel<T>(AuditEntityBaseEvent<T> eventMsg, dynamic parentEntity, CustomerInfo customer) where T : BaseEntity
        {
            return new AuditEntityLogModel
            {
                Id = eventMsg.Id,
                EventType = EventTypes.Create,
                CurrentEntity = eventMsg.CurrentEntity,
                Descriptions = eventMsg.Descriptions,
                EntityId = eventMsg.CurrentEntity.Id.ToString(),
                OccurredAt = eventMsg.OccurredAt,
                Customer = customer,
                ParentEntity = parentEntity,
            };
        }
        public static AuditEntityLogModel BuildDeleteLogModel<T>(AuditEntityBaseEvent<T> eventMsg, dynamic parentEntity, CustomerInfo customer) where T : BaseEntity
        {
            return new AuditEntityLogModel
            {
                Id = eventMsg.Id,
                EventType = EventTypes.Delete,
                CurrentEntity = eventMsg.CurrentEntity,
                Descriptions = eventMsg.Descriptions,
                EntityId = eventMsg.CurrentEntity.Id.ToString(),
                OccurredAt = eventMsg.OccurredAt,
                Customer = customer,
                ParentEntity = parentEntity,

            };
        }
    }
}
