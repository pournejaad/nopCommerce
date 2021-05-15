using System;
using System.Linq;
using System.Threading.Tasks;
using Leo.Core.Customers;
using LinqToDB;
using Nop.Core;
using Nop.Data;

namespace Leo.Service
{
    public interface IWalletService
    {
        Task DepositAsync(decimal amount);
        Task WithdrawAsync(decimal amount);
        Task<decimal> GetCustomerBalanceAsync();
    }

    public class WalletService : IWalletService
    {
        private readonly IRepository<Wallet> _walletRepository;
        private readonly IWorkContext _context;

        public WalletService(IRepository<Wallet> walletRepository, IWorkContext context)
        {
            _walletRepository = walletRepository;
            _context = context;
        }

        public async Task DepositAsync(decimal amount)
        {
            var customer = await _context.GetCurrentCustomerAsync();

            var mapping = await AsyncExtensions.FirstAsync(_walletRepository.Table, x => x.CustomerId == customer.Id);
            mapping.Balance += amount;
            await _walletRepository.UpdateAsync(mapping);
        }

        public async Task WithdrawAsync(decimal amount)
        {
            var customer = await _context.GetCurrentCustomerAsync();
            var mapping = await AsyncExtensions.FirstAsync(_walletRepository.Table, x => x.CustomerId == customer.Id);
            if (amount > mapping.Balance)
                throw new NopException($"No enough credit in customer {customer.Id} wallet.");
            mapping.Balance -= amount;
            await _walletRepository.UpdateAsync(mapping);
        }

        public async Task<decimal> GetCustomerBalanceAsync()
        {
            var customer = await _context.GetCurrentCustomerAsync();

            var wallet = await _walletRepository.Table
                .FirstOrDefaultAsync(x => x.CustomerId == customer.Id);
            if (wallet == null)
            {
                throw new NopException($"No wallet for customer: {customer.Id}");
            }

            return wallet.Balance;
        }
    }
}