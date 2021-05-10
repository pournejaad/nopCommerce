using Nop.Core;
using Nop.Web.Framework.Models;

namespace Ferasat.Plugin.Payments.CustomerClub.Models
{
    public class PaymentInfoModel
    {
        public bool UseCustomerClubWallet { get; set; }
    }

    public class ProductsWithPartialPayment : BaseEntity
    {
        public int ProductId { get; set; }
        public double Percentage { get; set; }
        public decimal MaximumAmount { get; set; }
    }
}