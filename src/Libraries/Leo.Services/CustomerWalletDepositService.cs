using System;
using System.Collections.Generic;
using System.Linq;
using Leo.Core.Domain;
using Nop.Core.Data;

namespace Leo.Services
{
    public class CustomerWalletDepositService : ICustomerWalletDepositService
    {
        private readonly IWalletService _walletService;
        private readonly IRepository<CustomerWalletDeposit> _customerWalletDepositRepository;

        public CustomerWalletDepositService(IWalletService walletService,
            IRepository<CustomerWalletDeposit> customerWalletDepositRepository)
        {
            _walletService = walletService;
            _customerWalletDepositRepository = customerWalletDepositRepository;
        }

        public void Execute(int customerId, decimal value)
        {
            var customerWalletDepositHistory = new CustomerWalletDeposit()
            {
                Amount = value,
                Source = "Customer Club",
                CreatedOn = DateTime.Now,
            };
            _customerWalletDepositRepository.Insert(customerWalletDepositHistory);
            _walletService.Deposit(customerId, value);
        }

        public IList<CustomerWalletDeposit> GetAll(string sourceName = null, decimal? amount = null, DateTime? depositDate = null)
        {
            var query = _customerWalletDepositRepository.TableNoTracking;
            if (!string.IsNullOrWhiteSpace(sourceName))
                query = query.Where(x => x.Source.Contains(sourceName));
            if (amount.HasValue)
                query = query.Where(x => x.Amount > amount.Value);
            if (depositDate.HasValue)
                query = query.Where(x => x.CreatedOn > depositDate.Value);
            return query.ToList();
        }
    }
}