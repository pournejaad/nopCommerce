using Nop.Core;

namespace Leo.Core.Domain
{
    public class PartialPaymentProductMapping : BaseEntity
    {
        public int PartialPaymentId { get; set; }

        public int ProductId { get; set; }
    }
}