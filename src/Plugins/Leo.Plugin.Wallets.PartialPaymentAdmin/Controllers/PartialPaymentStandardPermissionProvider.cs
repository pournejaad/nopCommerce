using Nop.Core.Domain.Security;

namespace Leo.Plugin.Wallets.PartialPaymentAdmin.Controllers
{
    public class PartialPaymentStandardPermissionProvider
    {
        public static readonly PermissionRecord ManagePartialPayments =
            new PermissionRecord() {Name = "Manage Partial Payments", SystemName = "ManagePartialPayments", Category = "Promo"};
    }
}