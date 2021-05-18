using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Security;

namespace Nop.Services.Security
{
    public partial class StandardPermissionProvider
    {
        public static readonly PermissionRecord ManagePartialPayments =
            new PermissionRecord { Name = "Manage Partial Payments", SystemName = "ManagePartialPayments", Category = "Promo" };
    }
}
