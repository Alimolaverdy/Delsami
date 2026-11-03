using Nop.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.SMS.Model;

namespace Delsami.Plugin.Misc.SMS.Data.Mapping
{
    internal class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(QueuedSMS), $"D_S_{nameof(QueuedSMS)}"},
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}
