using Nop.Core;
using Nop.Web.Framework.Models;

namespace Ferasat.Plugin.Customer.CustomWallet.Models
{
    public class Wallet : BaseEntity
    {
        public int CustomerId { get; set; }

        public decimal Balance { get; set; }
    }
}