using System;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;

namespace Leo.Plugin.Wallets.PartialPaymentAdmin.Models
{
    public class CustomerWalletDepositModel : BaseNopEntityModel
    {
        public decimal? Value { get; set; }
        [UIHint("DateNullable")] public DateTime? DepositedOn { get; set; }
        public string CustomerName { get; set; }
        public string SourceName { get; set; }
    }
}