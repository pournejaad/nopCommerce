using Nop.Web.Framework.Models;

namespace Leo.Plugin.Payments.CustomerWallet.Models
{
    public record PartialPaymentProductModel : BaseNopEntityModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }
}