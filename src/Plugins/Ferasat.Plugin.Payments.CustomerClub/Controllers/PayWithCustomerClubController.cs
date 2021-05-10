using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Controllers;

namespace Ferasat.Plugin.Payments.CustomerClub.Controllers
{
    public class PayWithCustomerClubController : BasePluginController
    {
        public IActionResult Index()
        {
            return View("~/Plugins/Payments.CustomerClub/Views/Index.cshtml");
        }
        
     
    }
}