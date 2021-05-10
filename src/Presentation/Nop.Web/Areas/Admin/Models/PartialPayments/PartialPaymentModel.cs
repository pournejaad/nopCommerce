using System;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.PartialPayments
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

        [NopResourceDisplayName("Admin.Promotions.PartialPayments.Fields.PartialPaymentPercentage")]
        public decimal PartialPaymentPercentage { get; set; }

        [NopResourceDisplayName("Admin.Promotions.PartialPayments.Fields.PartialPaymentAmount")]
        public decimal PartialPaymentAmount { get; set; }

        [NopResourceDisplayName("Admin.Promotions.PartialPayments.Fields.MaximumPartialPaymentAmount")]
        [UIHint("DecimalNullable")]
        public decimal? MaximumPartialPaymentAmount { get; set; }

        [NopResourceDisplayName("Admin.Promotions.PartialPayments.Fields.StartDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? StartDateUtc { get; set; }

        [NopResourceDisplayName("Admin.Promotions.PartialPayments.Fields.EndDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? EndDateUtc { get; set; }

        [NopResourceDisplayName("Admin.Promotions.PartialPayments.Requirements.RequirementGroup")]
        public int RequirementGroupId { get; set; }

        public PartialPaymentProductSearchModel PartialPaymentProductSearchModel { get; set; }
        #endregion
        
        
    }
}