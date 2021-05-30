using System;
using System.Collections.Generic;
using System.Linq;
using Leo.Core;
using Leo.Core.Domain;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;

namespace Leo.Services
{
    public interface IWalletService
    {
        void Deposit(int customerId, decimal amount);
        bool Withdraw(int customerId, decimal amount);
        decimal GetCustomerBalance(Customer customer);
        void WithdrawAppliedAmounts(Customer getCurrentCustomer);
        decimal GetAppliedValue(Customer customer);
        void ApplyPartialPayment(Customer customer, int storeId);
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

        public void Deposit(int customerId, decimal amount)
        {
            Wallet mapping = _walletRepository.Table.First(x => x.CustomerId == customerId);
            var currentValue = mapping.Balance;
            var finalValue = currentValue + amount;
            mapping.Balance = finalValue;
            _walletRepository.Update(mapping);
        }

        public bool Withdraw(int customerId, decimal amount)
        {
            var wallet = _walletRepository.Table.First(x => x.CustomerId == customerId);
            var currentValue = wallet.Balance;
            if (amount > currentValue)
                throw new NopException($"No enough credit in customer {customerId} wallet.");
            var finalValue = currentValue - amount;
            wallet.Balance = finalValue;
            try
            {
                _walletRepository.Update(wallet);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public decimal GetAppliedValue(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));
            return GetAppliedValue(customer, _store.CurrentStore.Id);
        }

        public void ApplyPartialPayment(Customer customer, int storeId)
        {
            var valueToApply = GetAppliedValue(customer, storeId);

            if (Withdraw(customer.Id, valueToApply))
            {
                _genericAttributeService.SaveAttribute<IList<PartialPaymentOption>>(customer,
                    GenericAttributeKeys.PartialPayment, null, storeId);
            }
        }

        public decimal GetAppliedValue(Customer customer, int storeId)
        {
            const decimal appliedAmount = decimal.Zero;

            var appliedPartialPayments = _genericAttributeService
                .GetAttribute<IList<PartialPaymentOption>>(customer,
                    GenericAttributeKeys.PartialPayment, storeId);

            if (appliedPartialPayments == null || !appliedPartialPayments.Any()) return appliedAmount;

            return appliedPartialPayments.Sum(option => option.AppliedValue);
        }

        public decimal GetCustomerBalance(Customer customer)
        {
            var wallet = _walletRepository.Table
                .FirstOrDefault(x => x.CustomerId == customer.Id);

            return wallet?.Balance ?? decimal.Zero;
        }

        public void WithdrawAppliedAmounts(Customer customer)
        {
            var amount = GetAppliedValue(customer);
            Withdraw(customer.Id, amount);
        }
    }
}