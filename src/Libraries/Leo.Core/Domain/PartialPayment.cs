using System;
using Nop.Core;

namespace Leo.Core.Domain
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