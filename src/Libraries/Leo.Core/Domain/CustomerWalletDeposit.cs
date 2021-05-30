using System;
using Nop.Core;

namespace Leo.Core.Domain
{
    public class CustomerWalletDeposit : BaseEntity
    {
        public string Source { get; set; }
        public decimal Amount { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}