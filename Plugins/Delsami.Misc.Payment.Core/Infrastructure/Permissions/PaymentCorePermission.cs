using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System.Collections.Generic;

namespace Delsami.Plugin.Payment.Core.Infrastructure.Permissions
{
    public class PaymentCorePermission : IPermissionProvider
    {
        public static readonly PermissionRecord ManagePayment = new PermissionRecord
        {
            Name = "Admin area. Manage Payment",
            SystemName = "ManagePayment",
            Category = "Plugin"
        };

        public IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                ManagePayment,
            };
        }


        public HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
        {
            return new HashSet<(string systemRoleName, PermissionRecord[] permissions)>
        {
            ("Administrators", new[] { ManagePayment }),

        };
        }
    }
}
