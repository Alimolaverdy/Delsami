using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Helpers
{
    public interface IEnumHelper
    {
        Task<string> GetDescriptionAsync(Enum value);
    }
}
