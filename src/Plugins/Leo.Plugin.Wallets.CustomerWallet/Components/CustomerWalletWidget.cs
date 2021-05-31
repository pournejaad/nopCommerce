using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Leo.Plugin.Wallets.CustomerWallet.Components
{
    [ViewComponent(Name="CustomerWalletWidget")]
    public class CustomerWalletWidgetComponent : NopViewComponent
    {
        public IViewComponentResult Invoke(string widgetZone)
        {
            return View("~/Plugins/Wallets.CustomerWallet/Views/_PartialPaymentBox.cshtml","Customer Wallet");
        }
    }
}