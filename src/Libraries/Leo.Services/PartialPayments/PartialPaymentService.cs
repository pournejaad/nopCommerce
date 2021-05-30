using System;
using System.Collections.Generic;
using System.Linq;
using Leo.Core.Domain;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;

namespace Leo.Services.PartialPayments
{
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

        public decimal GetPartialPaymentAmount(PartialPayment partialPayment, decimal amount)
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

        public void InsertPartialPaymentAsync(
            Core.Domain.PartialPayment partialPayment)
        {
            _partialPaymentRepository.Insert(partialPayment);
        }

        public PartialPayment GetPartialPaymentByIdAsync(int id)
        {
            return _partialPaymentRepository.GetById(id);
        }

        public IList<Product> GetProductsByIdsAsync(int[] productIds)
        {
            return _productRepository.Table
                .Where(x => productIds.Contains(x.Id))
                .ToList();
        }

        public PartialPaymentProductMapping
            GetPartialPaymentAppliedToProductAsync(int productId,
                int partialPaymentId)
        {
            return _partialPaymentProductMappingRepository.Table
                .FirstOrDefault(ppp =>
                    ppp.ProductId == productId &&
                    ppp.PartialPaymentId == partialPaymentId);
        }

        public void InsertPartialPaymentProductMappingAsync(
            PartialPaymentProductMapping partialPaymentProductMapping)
        {
            _partialPaymentProductMappingRepository.Insert(
                partialPaymentProductMapping);
        }

        public void UpdatePartialPaymentAsync(PartialPayment partialPayment)
        {
            _partialPaymentRepository.Update(partialPayment);
        }

        public IList<PartialPaymentProductMapping>
            GetAllPartialPaymentProductMappings()
        {
            return _partialPaymentProductMappingRepository
                .Table
                .ToList();
        }

        public PartialPaymentProductMapping
            GetPartialPaymentMappingByProductId(int productId)
        {
            return _partialPaymentProductMappingRepository
                .Table
                .FirstOrDefault(x => x.ProductId == productId);
        }

        public IList<PartialPaymentProductMapping>
            GetPartialPaymentMappingsByPartialPaymentId(int partialPaymentId)
        {
            return _partialPaymentProductMappingRepository
                .Table
                .Where(x => x.PartialPaymentId == partialPaymentId).ToList();
        }

        public void DeletePartialPaymentAsync(
            Core.Domain.PartialPayment partialPayment)
        {
        }

        public void DeletePartialPaymentProductMapping(
            PartialPaymentProductMapping partialPaymentProductMapping)
        {
            _partialPaymentProductMappingRepository.Delete(
                partialPaymentProductMapping);
        }

        public PartialPaymentValidationResult ValidatePartialPaymentAsync(PartialPayment partialPayment,
            Customer customer)
        {
            return new PartialPaymentValidationResult();
        }
    }
}