using System;
using System.Linq;
using Leo.Plugin.Wallets.PartialPaymentAdmin.Models;
using Leo.Services;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Web.Framework.Models.Extensions;

namespace Leo.Plugin.Wallets.PartialPaymentAdmin.Factories
{
    public class CustomerWalletDepositModelFactory : ICustomerWalletDepositModelFactory
    {
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICustomerWalletDepositService _customerWalletDepositService;
        private readonly ICustomerService _customerService;

        public CustomerWalletDepositModelFactory(IDateTimeHelper dateTimeHelper, ICustomerWalletDepositService customerWalletDepositService, ICustomerService customerService)
        {
            _dateTimeHelper = dateTimeHelper;
            _customerWalletDepositService = customerWalletDepositService;
            _customerService = customerService;
        }

        public CustomerWalletDepositSearchModel PrepareCustomerWalletDepositSearchModel(CustomerWalletDepositSearchModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.SetGridPageSize();

            return model;
        }

        public CustomerWalletDepositListModel PrepareCustomerWalletDepositListModel(CustomerWalletDepositSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            var sourceName = !string.IsNullOrWhiteSpace(searchModel.SearchSource) ? searchModel.SearchSource : null;
            var date = searchModel.SearchDepositDate.HasValue
                ? (DateTime?) _dateTimeHelper.ConvertToUtcTime(searchModel.SearchDepositDate.Value, _dateTimeHelper.CurrentTimeZone)
                : null;
            var amount = searchModel.SearchValue;
            var customerWalletDeposits = _customerWalletDepositService.GetAll(sourceName, amount, date).ToPagedList(searchModel);
            var model = new CustomerWalletDepositListModel().PrepareToGrid(searchModel, customerWalletDeposits, () =>
            {
                return customerWalletDeposits.Select(cwd =>
                {
                    var customerWalletDepositModel = new CustomerWalletDepositModel()
                    {
                        Id = cwd.Id,
                        Value = cwd.Amount,
                        CustomerName = _customerService.GetCustomerFullName(_customerService.GetCustomerById(cwd.CustomerId)),
                        SourceName = cwd.Source,
                        DepositedOn = cwd.CreatedOn
                    };
                    return customerWalletDepositModel;
                });
            });
            return model;
        }
    }
}