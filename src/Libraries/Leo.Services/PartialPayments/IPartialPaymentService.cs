using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Leo.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Nop.Core.Domain.Catalog;

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

        void InsertPartialPaymentAsync(Core.Domain.PartialPayment partialPayment);
        PartialPayment GetPartialPaymentByIdAsync(int id);
        IList<Product> GetProductsByIdsAsync(int[] productIds);

        PartialPaymentProductMapping GetPartialPaymentAppliedToProductAsync(
            int productId, int partialPaymentId);

        void InsertPartialPaymentProductMappingAsync(
            PartialPaymentProductMapping partialPaymentProductMapping);

        void UpdatePartialPaymentAsync(Core.Domain.PartialPayment partialPayment);

        IList<PartialPaymentProductMapping>
            GetAllPartialPaymentProductMappings();

        PartialPaymentProductMapping GetPartialPaymentMappingByProductId(
            int productId);

        IList<PartialPaymentProductMapping>
            GetPartialPaymentMappingsByPartialPaymentId(int partialPaymentId);

        void DeletePartialPaymentAsync(Core.Domain.PartialPayment partialPayment);

        void DeletePartialPaymentProductMapping(
            PartialPaymentProductMapping partialPaymentProductMapping);
    }
}