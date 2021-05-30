using Leo.Plugin.Wallets.PartialPaymentAdmin.Models;

namespace Leo.Plugin.Wallets.PartialPaymentAdmin.Factories
{
    public interface ICustomerWalletDepositModelFactory
    {
        CustomerWalletDepositSearchModel PrepareCustomerWalletDepositSearchModel(CustomerWalletDepositSearchModel model);
        CustomerWalletDepositListModel PrepareCustomerWalletDepositListModel(CustomerWalletDepositSearchModel model);
    }
}