using Nop.Web.Framework.Models;

namespace Leo.Plugin.Wallets.PartialPaymentAdmin.Models
{
    public class PartialPaymentProductModel : BaseNopEntityModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }
}