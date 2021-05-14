using System;
using Azure.Identity;
using Nop.Core;

namespace Leo.Core.Payments
{
    public partial class PartialPayment : BaseEntity
    {
        public string Name { get; set; }

        public bool UsePercentage { get; set; }

        public decimal PartialPaymentPercentage { get; set; }

        public decimal PartialPaymentAmount { get; set; }

        public decimal? MaximumPartialPaymentAmount { get; set; }

        public DateTime? StartDateUtc { get; set; }

        public DateTime? EndDateUtc { get; set; }
    }
}