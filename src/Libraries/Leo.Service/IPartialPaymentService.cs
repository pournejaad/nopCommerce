using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leo.Core.Payments;
using Nop.Core;
using Nop.Data;
using Nop.Services.Localization;

namespace Leo.Service
{
    public interface IPartialPaymentService
    {
        Task<IList<PartialPayment>> GetAllPartialPaymentsAsync(string partialPaymentName = null,
            bool showHidden = false,
            DateTime? startDateUtc = null, DateTime? endDateUtc = null);

        public decimal GetPartialPaymentAmount(PartialPayment partialPayment, decimal amount);

    }

    public class PartialPaymentService : IPartialPaymentService
    {
        private readonly IRepository<PartialPayment> _partialPaymentRepository;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;

        public PartialPaymentService(IRepository<PartialPayment> partialPaymentRepository, IStoreContext storeContext,
            ILocalizationService localizationService)
        {
            _partialPaymentRepository = partialPaymentRepository;
            _storeContext = storeContext;
            _localizationService = localizationService;
        }

        public async Task<IList<PartialPayment>> GetAllPartialPaymentsAsync(string partialPaymentName = null,
            bool showHidden = false,
            DateTime? startDateUtc = null, DateTime? endDateUtc = null)
        {
            var partialPayments = (await _partialPaymentRepository.GetAllAsync(query =>
            {
                if (!showHidden)
                {
                    query = query.Where(partialPayment =>
                        (!partialPayment.StartDateUtc.HasValue || partialPayment.StartDateUtc <= DateTime.UtcNow) &&
                        (!partialPayment.EndDateUtc.HasValue || partialPayment.EndDateUtc >= DateTime.UtcNow));
                }

                if (!string.IsNullOrEmpty(partialPaymentName))
                    query = query.Where(partialPayment => partialPayment.Name == partialPaymentName);

                query = query.OrderBy(pp => pp.Name).ThenBy(pp => pp.Id);

                return query;
            })).AsQueryable();

            if (startDateUtc.HasValue)
                partialPayments = partialPayments.Where(pp =>
                    !pp.StartDateUtc.HasValue || pp.StartDateUtc >= startDateUtc.Value);
            if (endDateUtc.HasValue)
                partialPayments = partialPayments.Where(discount =>
                    !discount.EndDateUtc.HasValue || discount.EndDateUtc <= endDateUtc.Value);

            return partialPayments.ToList();
        }

        public decimal GetPartialPaymentAmount(PartialPayment partialPayment, decimal amount)
        {
            if (partialPayment == null)
                throw new ArgumentNullException(nameof(partialPayment));
            decimal result;

            if (partialPayment.UsePercentage)
            {
                result = (decimal)((float)amount * (float)partialPayment.PartialPaymentPercentage / 100f);
                
            }
            else
            {
                result = partialPayment.PartialPaymentAmount;
            }

            if (partialPayment.UsePercentage &&
                partialPayment.MaximumPartialPaymentAmount.HasValue &&
                result > partialPayment.MaximumPartialPaymentAmount.Value)
                result = partialPayment.MaximumPartialPaymentAmount.Value;
            if (result < decimal.Zero)
                result = decimal.Zero;
            return result;

        }
    }
}