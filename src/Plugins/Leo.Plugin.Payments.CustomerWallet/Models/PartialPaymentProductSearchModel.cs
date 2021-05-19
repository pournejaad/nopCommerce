using Nop.Web.Framework.Models;

namespace Leo.Plugin.Payments.CustomerWallet.Models
{
    public record PartialPaymentProductSearchModel : BaseSearchModel
    {
        public int PartialPaymentId { get; set; }
    }
}