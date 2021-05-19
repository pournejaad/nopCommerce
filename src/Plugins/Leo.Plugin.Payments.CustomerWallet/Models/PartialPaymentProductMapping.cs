using Nop.Core;

namespace Leo.Plugin.Payments.CustomerWallet.Models
{
    public class PartialPaymentProductMapping : BaseEntity
    {
        public int PartialPaymentId { get; set; }
     
        public int ProductId { get; set; }
    }
}