using System;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;

namespace Leo.Plugin.Wallets.PartialPaymentAdmin.Models
{
    public class PartialPaymentUsageHistorySearchModel : BaseSearchModel
    {
        [UIHint("DateNullable")] public DateTime? PaymentDate { get; set; }

        public decimal? SpentValue { get; set; }
    }
}