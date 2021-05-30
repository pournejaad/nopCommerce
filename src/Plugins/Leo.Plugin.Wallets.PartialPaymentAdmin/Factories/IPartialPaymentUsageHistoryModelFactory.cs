using Leo.Plugin.Wallets.PartialPaymentAdmin.Models;

namespace Leo.Plugin.Wallets.PartialPaymentAdmin.Factories
{
    public interface IPartialPaymentUsageHistoryModelFactory
    {
        PartialPaymentUsageHistorySearchModel PrePartialPaymentUsageHistorySearchModel(PartialPaymentUsageHistorySearchModel model);
        PartialPaymentUsageHistoryListModel PreparePartialPaymentUsageHistoryListModel(PartialPaymentUsageHistorySearchModel searchModel);
    }
}