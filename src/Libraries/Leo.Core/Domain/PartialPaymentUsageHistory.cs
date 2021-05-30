using System;
using Nop.Core;

namespace Leo.Core.Domain
{
    public class PartialPaymentUsageHistory : BaseEntity
    {
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public decimal SpentValue { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}