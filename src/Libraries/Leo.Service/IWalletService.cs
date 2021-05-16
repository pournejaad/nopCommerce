using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leo.Core;
using Leo.Core.Customers;
using Leo.Core.Payments;
using LinqToDB;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Common;

namespace Leo.Service
{
    public interface IWalletService
    {
        Task DepositAsync(int customerId, decimal amount);
        Task WithdrawAsync(int customerId, decimal amount);
        Task<decimal> GetCustomerBalanceAsync(Customer customer);
        Task WithdrawAppliedAmounts(Customer getCurrentCustomer);
    }

    public class WalletService : IWalletService
    {
        private readonly IRepository<Wallet> _walletRepository;
        private readonly IWorkContext _context;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStoreContext _store;

        public WalletService(IRepository<Wallet> walletRepository, IWorkContext context, IGenericAttributeService genericAttributeService, IStoreContext store)
        {
            _walletRepository = walletRepository;
            _context = context;
            _genericAttributeService = genericAttributeService;
            _store = store;
        }

        public async Task DepositAsync(int customerId, decimal amount)
        {
            var customer = await _context.GetCurrentCustomerAsync();

            var mapping = await AsyncExtensions.FirstAsync(_walletRepository.Table, x => x.CustomerId == customer.Id);
            mapping.Balance += amount;
            await _walletRepository.UpdateAsync(mapping);
        }

        public async Task WithdrawAsync(int customerId, decimal amount)
        {
            var customer = await _context.GetCurrentCustomerAsync();
            var mapping = await AsyncExtensions.FirstAsync(_walletRepository.Table, x => x.CustomerId == customer.Id);
            if (amount > mapping.Balance)
                throw new NopException($"No enough credit in customer {customer.Id} wallet.");
            mapping.Balance -= amount;
            await _walletRepository.UpdateAsync(mapping);
        }

        public async Task<decimal> GetAppliedValue(Customer customer)
        {
            var appliedAmount = decimal.Zero;
            var appliedPartialPayments = await _genericAttributeService
                .GetAttributeAsync<IList<PartialPaymentOption>>(customer,
                    GenericAttributeKeys.PartialPayment,(await _store.GetCurrentStoreAsync()).Id);
            if (appliedPartialPayments == null || !appliedPartialPayments.Any()) return appliedAmount;

            return appliedPartialPayments.Sum(option => option.AppliedValue);
        }

        public async Task<decimal> GetCustomerBalanceAsync(Customer customer)
        {
            var wallet = await _walletRepository.Table
                .FirstOrDefaultAsync(x => x.CustomerId == customer.Id);
            var appliedPartialPayments = await _genericAttributeService
                .GetAttributeAsync<IList<PartialPaymentOption>>(customer,
                    GenericAttributeKeys.PartialPayment);
       

            if (wallet == null)
            {
                throw new NopException($"No wallet for customer: {customer.Id}");
            }

            return wallet.Balance;
        }

        public async Task WithdrawAppliedAmounts(Customer customer)
        {
            var amount = await GetAppliedValue(customer);
            await WithdrawAsync(customer.Id, amount);
            await DeleteAppliedAmounts(customer);
        }

        public async Task DeleteAppliedAmounts(Customer customer)
        {
            await _genericAttributeService.SaveAttributeAsync<IList<PartialPaymentOption>>(
                customer, GenericAttributeKeys.PartialPayment, null,
                (await _store.GetCurrentStoreAsync()).Id);
        }
    }
}