using Leo.Plugin.Widgets.CustomerWallet.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Controllers;

namespace Leo.Plugin.Widgets.CustomerWallet.Controllers
{
    public class PartiallyPayController : BasePluginController
    {
        [HttpPost]
        public IActionResult Apply(PartialPayWithWalletBoxModel model)
        {
            return View();
        }
    }
}