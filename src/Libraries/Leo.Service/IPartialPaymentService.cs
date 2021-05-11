using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leo.Core.Payments;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
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

        Task InsertPartialPaymentAsync(PartialPayment partialPayment);
        Task<PartialPayment> GetPartialPaymentByIdAsync(int id);
        Task UpdateDiscountAsync(PartialPayment partialPayment);
        Task<IList<Product>> GetProductsByIdsAsync(int[] productIds);
        Task<PartialPaymentProductMapping> GetPartialPaymentAppliedToProductAsync(int productId, int partialPaymentId);
        Task InsertPartialPaymentProductMappingAsync(PartialPaymentProductMapping partialPaymentProductMapping);
        Task UpdatePartialPaymentAsync(PartialPayment partialPayment);
    }

    public class PartialPaymentService : IPartialPaymentService
    {
        private readonly IRepository<PartialPayment> _partialPaymentRepository;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<PartialPaymentProductMapping> _partialPaymentProductMappingRepository;

        public PartialPaymentService(IRepository<PartialPayment> partialPaymentRepository, IStoreContext storeContext,
            ILocalizationService localizationService, IRepository<Product> productRepository,
            IRepository<PartialPaymentProductMapping> partialPaymentProductMappingRepository)
        {
            _partialPaymentRepository = partialPaymentRepository;
            _storeContext = storeContext;
            _localizationService = localizationService;
            _productRepository = productRepository;
            _partialPaymentProductMappingRepository = partialPaymentProductMappingRepository;
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

        public async Task InsertPartialPaymentAsync(PartialPayment partialPayment)
        {
            await _partialPaymentRepository.InsertAsync(partialPayment);
        }

        public async Task<PartialPayment> GetPartialPaymentByIdAsync(int id)
        {
            return await _partialPaymentRepository.GetByIdAsync(id, cache => default);
        }

        public virtual async Task UpdateDiscountAsync(PartialPayment partialPayment)
        {
            await _partialPaymentRepository.UpdateAsync(partialPayment);
        }

        public async Task<IList<Product>> GetProductsByIdsAsync(int[] productIds)
        {
            return await _productRepository.GetByIdsAsync(productIds, cache => default, false);
        }

        public async Task<PartialPaymentProductMapping> GetPartialPaymentAppliedToProductAsync(int productId,
            int partialPaymentId)
        {
            return await _partialPaymentProductMappingRepository.Table
                .FirstOrDefaultAsync(ppp => ppp.ProductId == productId && ppp.PartialPaymentId == partialPaymentId);
        }

        public async Task InsertPartialPaymentProductMappingAsync(
            PartialPaymentProductMapping partialPaymentProductMapping)
        {
            await _partialPaymentProductMappingRepository.InsertAsync(partialPaymentProductMapping);
        }

        public async Task UpdatePartialPaymentAsync(PartialPayment partialPayment)
        {
            await _partialPaymentRepository.UpdateAsync(partialPayment);
        }

        public async Task<PartialPaymentValidationResult> ValidatePartialPaymentAsync(PartialPayment partialPayment,
            Customer customer)
        {
            
        }
    }
}