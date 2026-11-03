using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Common;
using Nop.Services.Common;

namespace Delsami.Plugin.Widgets.AddressLocation.Services
{
    public interface IALAddressAttributeService : IAddressAttributeService
    {
        Task InsertLatAddressAttribute();

        Task InsertLngAddressAttribute();

        Task<AddressAttribute> GetAddressAttributeByNameAsync(string name);
    }
}
