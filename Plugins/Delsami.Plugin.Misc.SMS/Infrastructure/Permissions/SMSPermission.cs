using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System.Collections.Generic;

namespace Delsami.Plugin.Misc.SMS.Infrastructure.Permissions
{
    internal class SMSPermission : IPermissionProvider
    {
        public static readonly PermissionRecord ManageSMS = new PermissionRecord
        {
            Name = "Admin area. ManageSMS",
            SystemName = "ManageSMS",
            Category = "Plugin"
        };

        public IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                ManageSMS,
            };
        }
        public HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
        {
            return new HashSet<(string systemRoleName, PermissionRecord[] permissions)>
        {
            ("Administrators", new[] { ManageSMS }),

        };
        }
    }
}