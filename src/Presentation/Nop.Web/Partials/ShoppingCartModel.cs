using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.ShoppingCart
{
    public partial record ShoppingCartModel
    {
        public partial record PartialPayWithWalletBoxModel : BaseNopModel
        {
            public bool Display { get; set; }
            public decimal Amount { get; set; }
            public bool IsApplied { get; set; }
            public IList<string> Messages { get; set; }

            public PartialPayWithWalletBoxModel()
            {
                Messages = new List<string>();
            }
        }
    }
}