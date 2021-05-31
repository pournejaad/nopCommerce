using Leo.Plugin.Widgets.CustomerWallet.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Leo.Plugin.Widgets.CustomerWallet.Components
{
    [ViewComponent(Name = "WidgetsCustomerWallet")]
    public class WidgetsCustomerWalletViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke(string widgetZone)
        {
            return View("~/Plugins/Widgets.CustomerWallet/Views/PartialPaymentBox.cshtml",
                new PartialPayWithWalletBoxModel());
        }
    }
}