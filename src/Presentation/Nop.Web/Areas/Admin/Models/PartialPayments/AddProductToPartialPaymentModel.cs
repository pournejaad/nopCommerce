using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.PartialPayments
{
    public record AddProductToPartialPaymentModel : BaseNopModel
    {
        public AddProductToPartialPaymentModel()
        {
            SelectedProductIds = new List<int>();
        }
        public int PartialPaymentId { get; set; }
        public IList<int> SelectedProductIds { get; set; }
    }
}