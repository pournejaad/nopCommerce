using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leo.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;

namespace Leo.Services.PartialPayments
{
    public interface IPartialPaymentService
    {
        IList<PartialPayment> GetAllPartialPaymentsAsync(
            string partialPaymentName = null,
            bool showHidden = false,
            DateTime? startDateUtc = null, DateTime? endDateUtc = null);

        decimal GetPartialPaymentAmount(Core.Domain.PartialPayment partialPayment,
            decimal amount);

        Task InsertPartialPaymentAsync(Core.Domain.PartialPayment partialPayment);
        Task<Core.Domain.PartialPayment> GetPartialPaymentByIdAsync(int id);
        Task UpdateDiscountAsync(Core.Domain.PartialPayment partialPayment);
        Task<IList<Product>> GetProductsByIdsAsync(int[] productIds);

        Task<PartialPaymentProductMapping> GetPartialPaymentAppliedToProductAsync(
            int productId, int partialPaymentId);

        Task InsertPartialPaymentProductMappingAsync(
            PartialPaymentProductMapping partialPaymentProductMapping);

        Task UpdatePartialPaymentAsync(Core.Domain.PartialPayment partialPayment);

        Task<IList<PartialPaymentProductMapping>>
            GetAllPartialPaymentProductMappings();

        Task<PartialPaymentProductMapping> GetPartialPaymentMappingByProductId(
            int productId);

        Task<IList<PartialPaymentProductMapping>>
            GetPartialPaymentMappingsByPartialPaymentId(int partialPaymentId);

        Task DeletePartialPaymentAsync(Core.Domain.PartialPayment partialPayment);

        void DeletePartialPaymentProductMapping(
            PartialPaymentProductMapping partialPaymentProductMapping);
    }

    public class PartialPaymentService : IPartialPaymentService
    {
        private readonly IRepository<Core.Domain.PartialPayment>
            _partialPaymentRepository;

        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<Product> _productRepository;

        private readonly IRepository<PartialPaymentProductMapping>
            _partialPaymentProductMappingRepository;

        public PartialPaymentService(
            IRepository<Core.Domain.PartialPayment> partialPaymentRepository,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            IRepository<Product> productRepository,
            IRepository<PartialPaymentProductMapping>
                partialPaymentProductMappingRepository)
        {
            _partialPaymentRepository = partialPaymentRepository;
            _storeContext = storeContext;
            _localizationService = localizationService;
            _productRepository = productRepository;
            _partialPaymentProductMappingRepository =
                partialPaymentProductMappingRepository;
        }

        public IList<Core.Domain.PartialPayment> GetAllPartialPaymentsAsync(
            string partialPaymentName = null,
            bool showHidden = false,
            DateTime? startDateUtc = null,
            DateTime? endDateUtc = null)
        {
            var paymentQuery = _partialPaymentRepository.TableNoTracking;
            if (!showHidden)
            {
                paymentQuery = paymentQuery.Where(x =>
                    (!x.StartDateUtc.HasValue ||
                     x.StartDateUtc <= DateTime.UtcNow) &&
                    (!x.EndDateUtc.HasValue) || x.EndDateUtc >= DateTime.UtcNow);
            }

            if (!string.IsNullOrWhiteSpace(partialPaymentName))
                paymentQuery = paymentQuery
                    .Where(x => x.Name.Contains(partialPaymentName));

            paymentQuery = paymentQuery.OrderBy(x => x.Name).ThenBy(x => x.Id);

            if (startDateUtc.HasValue)
                paymentQuery = paymentQuery.Where(pp =>
                    !pp.StartDateUtc.HasValue ||
                    pp.StartDateUtc >= startDateUtc.Value);
            if (endDateUtc.HasValue)
                paymentQuery = paymentQuery.Where(partialPayment =>
                    !partialPayment.EndDateUtc.HasValue ||
                    partialPayment.EndDateUtc <= endDateUtc.Value);

            return paymentQuery.ToList();
        }

        public decimal GetPartialPaymentAmount(
            Core.Domain.PartialPayment partialPayment, decimal amount)
        {
            if (partialPayment == null)
                throw new ArgumentNullException(nameof(partialPayment));
            decimal result;

            if (partialPayment.UsePercentage)
            {
                result = (decimal) ((float) amount *
                    (float) partialPayment.PartialPaymentPercentage / 100f);
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

        public Task InsertPartialPaymentAsync(
            Core.Domain.PartialPayment partialPayment)
        {
            _partialPaymentRepository.Insert(partialPayment);
            return Task.CompletedTask;
        }

        public Task<Core.Domain.PartialPayment> GetPartialPaymentByIdAsync(int id)
        {
            return Task.FromResult(_partialPaymentRepository.GetById(id));
        }

        public virtual Task UpdateDiscountAsync(
            Core.Domain.PartialPayment partialPayment)
        {
            _partialPaymentRepository.Update(partialPayment);
            return Task.CompletedTask;
        }

        public async Task<IList<Product>> GetProductsByIdsAsync(int[] productIds)
        {
            return await _productRepository.Table
                .Where(x => productIds.Contains(x.Id))
                .ToListAsync();
        }

        public async Task<PartialPaymentProductMapping>
            GetPartialPaymentAppliedToProductAsync(int productId,
                int partialPaymentId)
        {
            return await _partialPaymentProductMappingRepository.Table
                .FirstOrDefaultAsync(ppp =>
                    ppp.ProductId == productId &&
                    ppp.PartialPaymentId == partialPaymentId);
        }

        public Task InsertPartialPaymentProductMappingAsync(
            PartialPaymentProductMapping partialPaymentProductMapping)
        {
            _partialPaymentProductMappingRepository.Insert(
                partialPaymentProductMapping);
            return Task.CompletedTask;
        }

        public Task UpdatePartialPaymentAsync(
            Core.Domain.PartialPayment partialPayment)
        {
            _partialPaymentRepository.Update(partialPayment);
            return Task.CompletedTask;
        }

        public async Task<IList<PartialPaymentProductMapping>>
            GetAllPartialPaymentProductMappings()
        {
            return await _partialPaymentProductMappingRepository
                .Table
                .ToListAsync();
        }

        public async Task<PartialPaymentProductMapping>
            GetPartialPaymentMappingByProductId(int productId)
        {
            return await _partialPaymentProductMappingRepository
                .Table
                .FirstOrDefaultAsync(x => x.ProductId == productId);
        }

        public async Task<IList<PartialPaymentProductMapping>>
            GetPartialPaymentMappingsByPartialPaymentId(int partialPaymentId)
        {
            return await _partialPaymentProductMappingRepository
                .Table
                .Where(x => x.PartialPaymentId == partialPaymentId).ToListAsync();
        }

        public Task DeletePartialPaymentAsync(
            Core.Domain.PartialPayment partialPayment)
        {
            return Task.CompletedTask;
        }

        public void DeletePartialPaymentProductMapping(
            PartialPaymentProductMapping partialPaymentProductMapping)
        {
            _partialPaymentProductMappingRepository.Delete(
                partialPaymentProductMapping);
        }

        public Task<PartialPaymentValidationResult> ValidatePartialPaymentAsync(
            Core.Domain.PartialPayment partialPayment,
            Customer customer)
        {
            return Task.FromResult(new PartialPaymentValidationResult());
        }
    }
}