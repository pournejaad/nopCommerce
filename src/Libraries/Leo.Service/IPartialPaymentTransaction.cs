using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.VisualBasic;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Data;
using Nop.Services.Common;

namespace Leo.Service
{
    public interface IPartialPaymentTransaction
    {
        Task PerformTransaction();
    }

    public class PartialPaymentTransaction : IPartialPaymentTransaction
    {
        private readonly IWorkContext _workContext;
        private readonly IWalletService _walletService;
        private readonly IStoreContext _storeContext;
        private readonly INopDataProvider _dataProvider;
        private readonly IGenericAttributeService _genericAttributeService;

        public PartialPaymentTransaction(IWorkContext context, IWalletService walletService, IStoreContext storeContext, INopDataProvider dataProvider,
            IGenericAttributeService genericAttributeService)
        {
            _workContext = context;
            _walletService = walletService;
            _storeContext = storeContext;
            _dataProvider = dataProvider;
            _genericAttributeService = genericAttributeService;
        }

        public async Task PerformTransaction()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            
            transaction.Complete();
            
            transaction.Dispose();
        }
    }
}