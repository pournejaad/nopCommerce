using System;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;

namespace Leo.Plugin.Wallets.PartialPaymentAdmin.Models
{
    public class CustomerWalletDepositSearchModel : BaseSearchModel
    {
        [UIHint("DateNullable")] public DateTime? SearchDepositDate { get; set; }
        public string SearchSource { get; set; }
        public decimal? SearchValue { get; set; }
    }
}