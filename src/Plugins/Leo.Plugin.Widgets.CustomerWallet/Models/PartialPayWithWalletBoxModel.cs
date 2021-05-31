using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Leo.Plugin.Widgets.CustomerWallet.Models
{
    public partial class PartialPayWithWalletBoxModel : BaseNopModel
    {
        public bool Display { get; set; }
        public decimal Amount { get; set; }
        public bool IsApplied { get; set; }
        public IList<string> Messages { get; set; }

        public PartialPayWithWalletBoxModel()
        {
            Messages = new List<string>();
        }
    }
}