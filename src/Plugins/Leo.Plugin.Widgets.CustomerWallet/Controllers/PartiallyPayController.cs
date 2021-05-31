using System;
using System.Collections.Generic;
using Leo.Core;
using Leo.Core.Domain;
using Leo.Plugin.Widgets.CustomerWallet.Models;
using Leo.Services;
using Leo.Services.PartialPayments;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Factories;
using Nop.Web.Models.ShoppingCart;
using Nop.Web.Framework.Controllers;

namespace Leo.Plugin.Widgets.CustomerWallet.Controllers
{
    public class PartiallyPayController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWalletService _walletService;
        private readonly IPartialPaymentService _partialPaymentService;
        private readonly IProductService _productService;
        private readonly ILocalizationService _localizationService;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;

        public PartiallyPayController(IWorkContext workContext, IPermissionService permissionService, INotificationService notificationService,
            IShoppingCartService shoppingCartService, IStoreContext storeContext, IGenericAttributeService genericAttributeService, IWalletService walletService,
            IPartialPaymentService partialPaymentService, IProductService productService, ILocalizationService localizationService,
            IShoppingCartModelFactory shoppingCartModelFactory)
        {
            _workContext = workContext;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _genericAttributeService = genericAttributeService;
            _walletService = walletService;
            _partialPaymentService = partialPaymentService;
            _productService = productService;
            _localizationService = localizationService;
            _shoppingCartModelFactory = shoppingCartModelFactory;
        }

        [HttpPost]
        public IActionResult Apply(PartialPayWithWalletBoxModel boxModel)
        {
            /*
            if (_workContext.CurrentCustomer.IsGuest())
            {
                return RedirectToRoute("LoginCheckoutAsGuest", new {returnUrl = Url.RouteUrl("ShoppingCart")});
            }

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
            {
                _notificationService.ErrorNotification("Only Customer Club Members can use Partial Payment");
                return RedirectToAction("Cart");
            }
            */

            var cart = _shoppingCartService
                .GetShoppingCart(_workContext.CurrentCustomer,
                    ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

            //   ParseAndSaveCheckoutAttributes(cart, form);

            var model = new ShoppingCartModel();

            _genericAttributeService.SaveAttribute<IList<PartialPaymentOption>>(_workContext.CurrentCustomer,
                GenericAttributeKeys.PartialPayment, null, _storeContext.CurrentStore.Id);

            var customerBalance = _walletService.GetCustomerBalance((_workContext.CurrentCustomer));
            var paymentOptions = new List<PartialPaymentOption>();
            var totalAllowedValueToPay = decimal.Zero;
            if (boxModel.Amount != decimal.Zero)
            {
                if (boxModel.Amount <= customerBalance)
                {
                    foreach (var item in cart)
                    {
                        var partialPaymentMapping = _partialPaymentService
                            .GetPartialPaymentMappingByProductId(item.ProductId);

                        if (partialPaymentMapping == null)
                            continue;

                        var partialPayment =
                            _partialPaymentService.GetPartialPaymentByIdAsync(
                                partialPaymentMapping.PartialPaymentId);
                        if (partialPayment == null)
                            continue;

                        var product = _productService
                            .GetProductById(partialPaymentMapping.ProductId);

                        var allowedValueToBePaidPartially = decimal.Zero;
                        if (partialPayment.UsePercentage)
                        {
                            var percent = product.Price *
                                partialPayment.PartialPaymentPercentage / 100M;

                            if (partialPayment.MaximumPartialPaymentAmount.HasValue &&
                                percent > partialPayment.MaximumPartialPaymentAmount.Value)
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
                        totalAllowedValueToPay += allowedValue;
                        var appliedValue = decimal.Zero;
                        if (boxModel.Amount > allowedValue)
                        {
                            boxModel.Amount -= allowedValue;
                            appliedValue = allowedValue;
                        }
                        else
                        {
                            appliedValue = boxModel.Amount;
                            boxModel.Amount = decimal.Zero;
                        }


                        var paymentOption = new PartialPaymentOption()
                        {
                            ShoppingCartItemId = item.Id,
                            AllowedValue = allowedValue,
                            AppliedValue = appliedValue
                        };
                        paymentOptions.Add(paymentOption);
                    }


                    var customer = _workContext.CurrentCustomer;
                    var store = _storeContext.CurrentStore;

                    _genericAttributeService.SaveAttribute<IList<PartialPaymentOption>>(customer,
                        GenericAttributeKeys.PartialPayment, paymentOptions, store.Id);

                    // get products in cart.
                    // get the ones with partial payment 
                    // for each product calculate the maximum amount that can be reduced.
                    // for each product reduce the amount till amount satisfies.
                }
                else
                {
                    // model.PartialPayWithWalletBox.Messages.Add($"not enough credit in your wallet, you've got {customerBalance}");
                }
            }
            else
                // model.PartialPayWithWalletBox.Messages.Add(
                // _localizationService.GetResource(
                // "ShoppingCart.PartialPaymentAmount.Empty"));

                model = _shoppingCartModelFactory
                    .PrepareShoppingCartModel(model, cart);

            return RedirectToRoute("cart");
        }

        #region Utility

        private decimal CalculateTotalAllowedValue(IList<ShoppingCartItem> cart)
        {
            var totalAllowedValueToPay = decimal.Zero;

            foreach (var item in cart)
            {
                var partialPayment = ActivePartialPayment(item.ProductId);
                if (partialPayment == null)
                    continue;

                var product = _productService
                    .GetProductById(item.ProductId);

                var allowedValueToBePaidPartially = decimal.Zero;
                if (partialPayment.UsePercentage)
                {
                    var percent = product.Price *
                        partialPayment.PartialPaymentPercentage / 100M;

                    if (partialPayment.MaximumPartialPaymentAmount.HasValue &&
                        percent > partialPayment.MaximumPartialPaymentAmount.Value)
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
                totalAllowedValueToPay += allowedValue;
            }

            return totalAllowedValueToPay;
        }

        public PartialPayment ActivePartialPayment(int productId)
        {
            var partialPaymentMapping = _partialPaymentService
                .GetPartialPaymentMappingByProductId(productId);

            if (partialPaymentMapping == null)
                return null;

            var partialPayment = _partialPaymentService.GetPartialPaymentByIdAsync(
                partialPaymentMapping.PartialPaymentId);
            if (partialPayment == null)
                return null;
            if ((partialPayment.StartDateUtc.HasValue && partialPayment.StartDateUtc.Value > DateTime.Now) ||
                (partialPayment.EndDateUtc.HasValue && partialPayment.EndDateUtc < DateTime.Now))
                return null;

            return partialPayment;
        }
        public decimal CalculateAllowedValueForProduct
        #endregion
    }
}