using System;
using System.Threading.Tasks;
using Ferasat.Plugin.Customer.CustomWallet.Models;
using LinqToDB;
using Nop.Core;
using Nop.Data;

namespace Ferasat.Plugin.Customer.CustomWallet.Services
{
    public interface IWalletService
    {
        Task Deposit(decimal amount);
        Task Withdraw(decimal amount);
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

        public async Task Deposit(decimal amount)
        {
            var customer = await _context.GetCurrentCustomerAsync();

            var mapping = await _walletRepository.Table
                .FirstAsync(x => x.CustomerId == customer.Id);
            mapping.Balance += amount;
            await _walletRepository.UpdateAsync(mapping);
        }

        public async Task Withdraw(decimal amount)
        {
            var customer = await _context.GetCurrentCustomerAsync();
            var mapping = await _walletRepository.Table.FirstAsync(x => x.CustomerId == customer.Id);
            if (amount > mapping.Balance)
                throw new Exception("No enough credit");
            mapping.Balance -= amount;
            await _walletRepository.UpdateAsync(mapping);
        }
    }
}