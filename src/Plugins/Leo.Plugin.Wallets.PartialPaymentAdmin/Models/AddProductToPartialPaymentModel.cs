using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Leo.Plugin.Wallets.PartialPaymentAdmin.Models
{
    public class AddProductToPartialPaymentModel : BaseNopModel
    {
        public AddProductToPartialPaymentModel()
        {
            SelectedProductIds = new List<int>();
        }
        public int PartialPaymentId { get; set; }
        public IList<int> SelectedProductIds { get; set; }
    }
}