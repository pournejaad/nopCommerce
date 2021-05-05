using System.Threading.Tasks;
using Ferasat.Plugin.Customer.CustomWallet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Ferasat.Plugin.Customer.CustomWallet.Controllers
{
    public class CustomWalletController : BasePluginController
    {
        private readonly IWalletService _walletService;

        public CustomWalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

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

        [HttpPost]
        public async Task<IActionResult> WithdrawAsync(decimal amount)
        {
            await _walletService.Withdraw(amount);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DepositAsync(decimal amount)
        {
            await _walletService.Deposit(amount);
            return Ok();
        }
    }
}