using Nop.Core;

namespace Leo.Core.Domain
{
    public class Wallet : BaseEntity
    {
        public int CustomerId { get; set; }
        public decimal Balance { get; set; }
    }
}
