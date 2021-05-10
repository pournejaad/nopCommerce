using System;
using System.Linq;
using System.Threading.Tasks;
using Leo.Core.Payments;
using Leo.Service;
using Nop.Services.Helpers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.PartialPayments;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Factories.PartialPayments
{
    public interface IPartialPaymentModelFactory
    {
        Task<PartialPaymentSearchModel> PreparePartialPaymentSearchModelAsync(PartialPaymentSearchModel model);
        Task<PartialPaymentListModel> PreparePartialPaymentListModelAsync(PartialPaymentSearchModel searchModel);

        Task<PartialPaymentModel> PreparePartialPaymentModelAsync(PartialPaymentModel partialPaymentModel,
            PartialPayment partialPayment);
    }

    public class PartialPaymentModelFactory : IPartialPaymentModelFactory
    {
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPartialPaymentService _partialPaymentService;

        public PartialPaymentModelFactory(IDateTimeHelper dateTimeHelper, IPartialPaymentService partialPaymentService)
        {
            _dateTimeHelper = dateTimeHelper;
            _partialPaymentService = partialPaymentService;
        }

        public async Task<PartialPaymentSearchModel> PreparePartialPaymentSearchModelAsync(
            PartialPaymentSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            model.SetGridPageSize();

            return model;
        }

        public async Task<PartialPaymentListModel> PreparePartialPaymentListModelAsync(
            PartialPaymentSearchModel searchModel)
        {
            var startDateUtc = searchModel.SearchStartDate.HasValue
                ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchStartDate.Value,
                    await _dateTimeHelper.GetCurrentTimeZoneAsync())
                : null;
            var endDateUtc = searchModel.SearchEndDate.HasValue
                ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchEndDate.Value,
                    await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1)
                : null;

            var partialPayments = (await _partialPaymentService.GetAllPartialPaymentsAsync(showHidden: true,
                    partialPaymentName: searchModel.SearchPartialPaymentName,
                    startDateUtc: startDateUtc,
                    endDateUtc: endDateUtc))
                .ToPagedList(searchModel);
            var model = await new PartialPaymentListModel().PrepareToGridAsync(searchModel,
                partialPayments, () =>
                {
                    return partialPayments.SelectAwait(async pp =>
                    {
                        var pPModel = pp.ToModel<PartialPaymentModel>();
                        return pPModel;
                    });
                });
            return model;
        }

        public async Task<PartialPaymentModel> PreparePartialPaymentModelAsync(PartialPaymentModel model,
            PartialPayment partialPayment)
        {
            if (partialPayment != null)
            {
                model ??= partialPayment.ToModel<PartialPaymentModel>();
                
                // apply rules
                PreparePartialPaymentProductSearchModel(model.PartialPaymentProductSearchModel, partialPayment);
            }
            
            return model;
        }

        private PartialPaymentProductSearchModel PreparePartialPaymentProductSearchModel(PartialPaymentProductSearchModel searchModel, PartialPayment partialPayment)
        {
            if (searchModel == null)
            {
                throw new ArgumentNullException(nameof(searchModel));
            }

            if (partialPayment == null)
            {
                throw new ArgumentNullException(nameof(partialPayment));
            }

            searchModel.SetGridPageSize();
            return searchModel;
        }
    }
}