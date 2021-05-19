using Nop.Core;

namespace Leo.Plugin.Payments.CustomerWallet.Models
{
    public class Wallet : BaseEntity
    {
        public int CustomerId { get; set; }

        public decimal Balance { get; set; }
    }
}