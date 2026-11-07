using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System.Collections.Generic;

namespace Delsami.Plugin.Shipping.DelsamiLogistic.Infrastructures.Permissions
{
    public class DelsamiLogisticPermission : IPermissionProvider
    {
        public static readonly PermissionRecord ManageDelsamiLogistic = new PermissionRecord
        {
            Name = "Admin area. ManageDelsamiLogistic",
            SystemName = "ManageDelsamiLogistic",
            Category = "Plugin"
        };

        public IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                ManageDelsamiLogistic,
            };
        }
        public HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
        {
            return new HashSet<(string systemRoleName, PermissionRecord[] permissions)>
        {
            ("Administrators", new[] { ManageDelsamiLogistic }),

        };
        }
    }
}
