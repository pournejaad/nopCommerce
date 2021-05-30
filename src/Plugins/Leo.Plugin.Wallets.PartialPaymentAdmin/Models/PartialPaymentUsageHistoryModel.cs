using System;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;

namespace Leo.Plugin.Wallets.PartialPaymentAdmin.Models
{
    public class PartialPaymentUsageHistoryModel : BaseNopEntityModel
    {
        public string CustomerName { get; set; }
        [UIHint("DateNullable")] public DateTime? PaymentDate { get; set; }
        public int OrderId { get; set; }
        public decimal? SpentValue { get; set; }
    }
}