using System;
using System.Collections.Generic;
using System.Linq;
using Leo.Core.Domain;
using Nop.Core.Data;

namespace Leo.Services
{
    public interface ICustomerWalletDepositService
    {
        void Execute(int customerId, decimal value);
        IList<CustomerWalletDeposit> GetAll(string sourceName = null, decimal? amount = null, DateTime? depositDate = null);
    }

    public interface IPartialPaymentUsageHistoryService
    {
        IList<PartialPaymentUsageHistory> GetAll(decimal? spentValue, DateTime? orderDate);
    }

    public class PartialPaymentUsageHistoryService : IPartialPaymentUsageHistoryService
    {
        private readonly IRepository<PartialPaymentUsageHistory> _partialPaymentUsageHistory;

        public PartialPaymentUsageHistoryService(IRepository<PartialPaymentUsageHistory> partialPaymentUsageHistory)
        {
            _partialPaymentUsageHistory = partialPaymentUsageHistory;
        }

        public IList<PartialPaymentUsageHistory> GetAll(decimal? spentValue, DateTime? orderDate)
        {
            var query = _partialPaymentUsageHistory.TableNoTracking;
            if (spentValue.HasValue)
                query = query.Where(x => x.SpentValue > spentValue);
            if (orderDate.HasValue)
                query = query.Where(x => x.CreatedOn > orderDate);
            return query.ToList();
        }
    }
}