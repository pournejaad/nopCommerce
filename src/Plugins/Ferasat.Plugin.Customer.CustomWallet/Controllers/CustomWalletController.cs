using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Ferasat.Plugin.Customer.CustomWallet.Controllers
{
    public class CustomWalletController : BasePluginController
    {
        public IActionResult Index()
        {
            return View("~/Plugins/Customer.CustomWallet/Views/Index.cshtml");
        }
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            return View("~/Plugins/Customer.CutomWallet/Views/Configure.cshtml");
        }

        [HttpPost]
        public IActionResult Configure(string model)
        {
            return Configure();
        }
        
    }
}