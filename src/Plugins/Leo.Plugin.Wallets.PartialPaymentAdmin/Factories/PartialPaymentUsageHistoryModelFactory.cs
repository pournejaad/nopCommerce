using System;
using System.Linq;
using Leo.Plugin.Wallets.PartialPaymentAdmin.Models;
using Leo.Services;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Web.Framework.Models.Extensions;

namespace Leo.Plugin.Wallets.PartialPaymentAdmin.Factories
{
    public class PartialPaymentUsageHistoryModelFactory : IPartialPaymentUsageHistoryModelFactory
    {
        private readonly IPartialPaymentUsageHistoryService _partialPaymentUsageHistoryService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICustomerService _customerService;
    
        public PartialPaymentUsageHistoryModelFactory(IPartialPaymentUsageHistoryService partialPaymentUsageHistoryService, IDateTimeHelper dateTimeHelper,
            ICustomerService customerService)
        {
            _partialPaymentUsageHistoryService = partialPaymentUsageHistoryService;
            _dateTimeHelper = dateTimeHelper;
            _customerService = customerService;
        }
    
        public PartialPaymentUsageHistorySearchModel PrePartialPaymentUsageHistorySearchModel(PartialPaymentUsageHistorySearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
    
            model.SetGridPageSize();
            return model;
        }
    
        public PartialPaymentUsageHistoryListModel PreparePartialPaymentUsageHistoryListModel(
            PartialPaymentUsageHistorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            var spentValue = searchModel.SpentValue;
            var date = searchModel.PaymentDate.HasValue
                ? (DateTime?) _dateTimeHelper.ConvertToUtcTime(searchModel.PaymentDate.Value, _dateTimeHelper.CurrentTimeZone)
                : null;
            var paymentUsageHistories = _partialPaymentUsageHistoryService.GetAll(spentValue, date).ToPagedList(searchModel);
            var model = new PartialPaymentUsageHistoryListModel().PrepareToGrid(searchModel, paymentUsageHistories, () =>
            {
                return paymentUsageHistories.Select(puh =>
                {
                    var usageHistoryModel = new PartialPaymentUsageHistoryModel()
                    {
                        Id = puh.Id,
                        CustomerName = _customerService.GetCustomerFullName(_customerService.GetCustomerById(puh.CustomerId)),
                        OrderId = puh.OrderId,
                        PaymentDate = _dateTimeHelper.ConvertToUserTime(puh.CreatedOn, _dateTimeHelper.CurrentTimeZone),
                        SpentValue = puh.SpentValue
                    };
                    return usageHistoryModel;
                });
            });
            return model;
        }
    }
}