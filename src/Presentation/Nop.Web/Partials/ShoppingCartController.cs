using System.Collections.Generic;
using System.Threading.Tasks;
using Leo.Core;
using Leo.Core.Payments;
using Leo.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Web.Controllers
{
    public partial class ShoppingCartController
    {
        private readonly IWalletService _walletService;

        [HttpPost, ActionName("Cart")]
        [FormValueRequired("removepartialpayment")]
        public virtual async Task<IActionResult> RemovePartial(IFormCollection form)
        {
            await _genericAttributeService.SaveAttributeAsync<IList<PartialPaymentOption>>(
                await _workContext.GetCurrentCustomerAsync(), GenericAttributeKeys.PartialPayment,
                null, (await _storeContext.GetCurrentStoreAsync()).Id);
            var model = new ShoppingCartModel();

            var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(),
                ShoppingCartType.ShoppingCart,
                (await _storeContext.GetCurrentStoreAsync()).Id);
            model = await _shoppingCartModelFactory
                .PrepareShoppingCartModelAsync(model, cart);
            return View(model);
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired("applypartialpayment")]
        public virtual async Task<IActionResult> ApplyPartial(string amount,
            IFormCollection form)
        {
            var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(),
                ShoppingCartType.ShoppingCart,
                (await _storeContext.GetCurrentStoreAsync()).Id);
            await ParseAndSaveCheckoutAttributesAsync(cart, form);
            var model = new ShoppingCartModel();
            await _genericAttributeService.SaveAttributeAsync<IList<PartialPaymentOption>>(await _workContext.GetCurrentCustomerAsync(), GenericAttributeKeys.PartialPayment, null,
                (await _storeContext.GetCurrentStoreAsync()).Id);

            var customerBalance =
                await _walletService.GetCustomerBalanceAsync((await _workContext.GetCurrentCustomerAsync()));
            var paymentOptions = new List<PartialPaymentOption>();
            if (!string.IsNullOrWhiteSpace(amount))
            {
                if (decimal.TryParse(amount, out var customerAmount))
                {
                    if (customerAmount <= customerBalance)
                    {
                        foreach (var item in cart)
                        {
                            var partialPaymentMapping = await _partialPaymentService
                                .GetPartialPaymentMappingByProductId(item.ProductId);

                            if (partialPaymentMapping == null)
                                continue;

                            var partialPayment =
                                await _partialPaymentService.GetPartialPaymentByIdAsync(
                                    partialPaymentMapping
                                        .PartialPaymentId);
                            if (partialPayment == null)
                                continue;

                            var product = await _productService
                                .GetProductByIdAsync(partialPaymentMapping.ProductId);

                            var allowedValueToBePaidPartially = decimal.Zero;
                            if (partialPayment.UsePercentage)
                            {
                                var percent = product.Price *
                                    partialPayment.PartialPaymentPercentage / 100M;

                                if (partialPayment.MaximumPartialPaymentAmount
                                        .HasValue &&
                                    percent > partialPayment.MaximumPartialPaymentAmount
                                        .Value)
                                {
                                    allowedValueToBePaidPartially = partialPayment
                                        .MaximumPartialPaymentAmount.Value;
                                }
                                else
                                {
                                    allowedValueToBePaidPartially = percent;
                                }
                            }
                            else
                            {
                                allowedValueToBePaidPartially =
                                    partialPayment.PartialPaymentAmount;
                            }

                            var quantity = item.Quantity;
                            var allowedValue = allowedValueToBePaidPartially * quantity;
                            var appliedValue = decimal.Zero;
                            if (customerAmount > allowedValue)
                            {
                                customerAmount -= allowedValue;
                                appliedValue = allowedValue;
                            }
                            else
                            {
                                appliedValue = customerAmount;
                                customerAmount = decimal.Zero;
                            }


                            var paymentOption = new PartialPaymentOption() {ShoppingCarteItemId = item.Id, AllowedValue = allowedValue, AppliedValue = appliedValue};
                            paymentOptions.Add(paymentOption);
                        }


                        var customer = await _workContext.GetCurrentCustomerAsync();
                        var store = await _storeContext.GetCurrentStoreAsync();

                        await _genericAttributeService.SaveAttributeAsync<IList<PartialPaymentOption>>(customer,
                            GenericAttributeKeys.PartialPayment, paymentOptions, store.Id);

                        // get products in cart.
                        // get the ones with partial payment 
                        // for each product calculate the maximum amount that can be reduced.
                        // for each product reduce the amount till amount satisfies.
                    }
                    else
                    {
                        model.PartialPayWithWalletBox.Messages.Add("not enough credit in your wallet");
                    }
                }
                else
                {
                    model.PartialPayWithWalletBox.Messages.Add("Should enter amount");
                }
            }
            else
                model.PartialPayWithWalletBox.Messages.Add(
                    await _localizationService.GetResourceAsync(
                        "ShoppingCart.PartialPaymentAmount.Empty"));

            model = await _shoppingCartModelFactory
                .PrepareShoppingCartModelAsync(model, cart);

            return View(model);
        }
    }
}