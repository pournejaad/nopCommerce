using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.PartialPayments
{
    public record PartialPaymentProductSearchModel : BaseSearchModel
    {
        public int PartialPaymentId { get; set; }
    }
}