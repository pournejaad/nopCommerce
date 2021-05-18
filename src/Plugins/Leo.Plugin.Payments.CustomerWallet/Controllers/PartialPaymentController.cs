using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Plugins;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Menu;
using Nop.Web.Framework.Mvc.Filters;

namespace Leo.Plugin.Payments.CustomerWallet.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class PartialPaymentController : BasePluginController
    {
        public IActionResult List()
        {
            return View("~/Plugins/Payments.CustomerWallet/Views/List.cshtml");
        }
    }
}