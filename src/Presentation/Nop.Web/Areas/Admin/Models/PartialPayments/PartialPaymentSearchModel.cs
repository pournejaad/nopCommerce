using System;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.PartialPayments
{
    public record PartialPaymentSearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Admin.Promotions.PartialPayment.List.SearchName")]
        public string SearchPartialPaymentName { get; set; }

        [NopResourceDisplayName("Admin.Promotions.PartialPayment.List.SearchStartDate")]
        [UIHint("DateNullable")]
        public DateTime? SearchStartDate { get; set; }

        [NopResourceDisplayName("Admin.Promotions.PartialPayment.List.SearchEndDate")]
        [UIHint("DateNullable")]
        public DateTime? SearchEndDate { get; set; }
    }

    public record PartialPaymentProductModel : BaseNopEntityModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }
}