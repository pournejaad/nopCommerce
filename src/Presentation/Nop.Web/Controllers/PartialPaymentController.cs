using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leo.Core;
using Leo.Core.Payments;
using Leo.Service;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Common;

namespace Nop.Web.Controllers
{
    public class PartialPaymentController : BasePublicController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWalletService _walletService;

        public PartialPaymentController(IWorkContext workContext, IStoreContext storeContext, IGenericAttributeService genericAttributeService, IWalletService walletService)
        {
            _workContext = workContext;
            _storeContext = storeContext;
            _genericAttributeService = genericAttributeService;
            _walletService = walletService;
        }

        public async Task<IActionResult> GetTotalAppliedAmount()
        {
            var partialPayments = await _genericAttributeService.GetAttributeAsync<IList<PartialPaymentOption>>(
                await _workContext.GetCurrentCustomerAsync(), GenericAttributeKeys.PartialPayment);
            var total = decimal.Zero;
            if (partialPayments == null || !partialPayments.Any()) return Json(new {amount = total});
            total = partialPayments.Sum(ppo => ppo.AppliedValue);

            return Json(new {applied = total});
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentBalance()
        {
            var currentBalance = await _walletService
                .GetCustomerBalanceAsync(await _workContext.GetCurrentCustomerAsync());
            return Json(new {balance = currentBalance});
        }
    }
}