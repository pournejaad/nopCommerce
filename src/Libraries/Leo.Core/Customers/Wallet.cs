using Nop.Core;

namespace Leo.Core.Customers
{
    public class Wallet : BaseEntity
    {
        public int CustomerId { get; set; }

        public decimal Balance { get; set; }
    }
}