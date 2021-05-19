using System;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Leo.Plugin.Payments.CustomerWallet.Models
{
    public record PartialPaymentModel : BaseNopEntityModel
    {
        public PartialPaymentModel()
        {
            PartialPaymentProductSearchModel = new PartialPaymentProductSearchModel();
        }

        #region Properties

        [NopResourceDisplayName("Admin.Promotions.PartialPayments.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Promotions.PartialPayments.Fields.AdminComment")]
        public string AdminComment { get; set; }

        [NopResourceDisplayName("Admin.Promotions.PartialPayments.Fields.UsePercentage")]
        public bool UsePercentage { get; set; }

        [NopResourceDisplayName("Admin.Promotions.PartialPayments.Fields.PartialPaymentPercentage")]
        public decimal PartialPaymentPercentage { get; set; }

        [NopResourceDisplayName("Admin.Promotions.PartialPayments.Fields.PartialPaymentAmount")]
        public decimal PartialPaymentAmount { get; set; }

        [NopResourceDisplayName("Admin.Promotions.PartialPayments.Fields.MaximumPartialPaymentAmount")]
        public decimal? MaximumPartialPaymentAmount { get; set; }

        [NopResourceDisplayName("Admin.Promotions.PartialPayments.Fields.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDateUtc { get; set; }

        [NopResourceDisplayName("Admin.Promotions.PartialPayments.Fields.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDateUtc { get; set; }

        public PartialPaymentProductSearchModel PartialPaymentProductSearchModel { get; set; }

        #endregion
    }
}