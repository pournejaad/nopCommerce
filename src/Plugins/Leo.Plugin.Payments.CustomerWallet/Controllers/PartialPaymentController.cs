using System;
using System.Linq;
using System.Threading.Tasks;
using Leo.Plugin.Payments.CustomerWallet.Factories;
using Leo.Plugin.Payments.CustomerWallet.Models;
using Leo.Plugin.Payments.CustomerWallet.Services;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Leo.Plugin.Payments.CustomerWallet.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class PartialPaymentController : BasePluginController
    {
        private readonly IPartialPaymentModelFactory _partialPaymentModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly IPartialPaymentService _partialPaymentService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IProductService _productService;

        public PartialPaymentController(IPartialPaymentModelFactory partialPaymentModelFactory,
            IPermissionService permissionService,
            IPartialPaymentService partialPaymentService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IProductService productService)
        {
            _partialPaymentModelFactory = partialPaymentModelFactory;
            _permissionService = permissionService;
            _partialPaymentService = partialPaymentService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _productService = productService;
        }

        public async Task<IActionResult> List()
        {
            var model = await _partialPaymentModelFactory.PreparePartialPaymentSearchModelAsync(new PartialPaymentSearchModel());
            return View("~/Plugins/Payments.CustomerWallet/Views/List.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> List(PartialPaymentSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePartialPayments))
                return await AccessDeniedDataTablesJson();

            var model = await _partialPaymentModelFactory.PreparePartialPaymentListModelAsync(searchModel);
            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePartialPayments))
                return AccessDeniedView();

            var model = await _partialPaymentModelFactory.PreparePartialPaymentModelAsync(new PartialPaymentModel(),
                null);
            return View("~/Plugins/Payments.CustomerWallet/Views/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(PartialPaymentModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePartialPayments))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var partialPayment = model.ToEntity<PartialPayment>();
                await _partialPaymentService.InsertPartialPaymentAsync(partialPayment);

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewPartialPayment",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewPartialPayment"),
                        partialPayment.Name), partialPayment);

                _notificationService.SuccessNotification(
                    await _localizationService.GetResourceAsync("Admin.Promotions.PartialPayments.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new {id = partialPayment.Id});
            }

            //prepare model
            model = await _partialPaymentModelFactory.PreparePartialPaymentModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View("~/Plugins/Payments.CustomerWallet/Views/Create.cshtml", model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePartialPayments))
                return AccessDeniedView();
            //try to get a discount with the specified id
            var partialPayment = await _partialPaymentService.GetPartialPaymentByIdAsync(id);
            if (partialPayment == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _partialPaymentModelFactory.PreparePartialPaymentModelAsync(null, partialPayment);

            return View("~/Plugins/Payments.CustomerWallet/Views/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(PartialPaymentModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePartialPayments))
                return AccessDeniedView();
            //try to get a partialpayment with the specified id
            var partialPayment = await _partialPaymentService.GetPartialPaymentByIdAsync(model.Id);
            if (partialPayment == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                partialPayment = model.ToEntity(partialPayment);
                await _partialPaymentService.UpdatePartialPaymentAsync(partialPayment);

                //activity log
                await _customerActivityService.InsertActivityAsync("EditPartialPayment",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EdiPartialPayment"), partialPayment.Name), partialPayment);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.PartialPayments.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new {id = partialPayment.Id});
            }

            //prepare model
            model = await _partialPaymentModelFactory.PreparePartialPaymentModelAsync(model, partialPayment, true);

            //if we got this far, something failed, redisplay form
            return View("~/Plugins/Payments.CustomerWallet/Views/Edit.cshtml", model);
        }

        public async Task<IActionResult> ProductDelete(int partialPaymentId, int productId)
        {
            if (!(await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePartialPayments)))
                return AccessDeniedView();
            var partialPayment = await _partialPaymentService.GetPartialPaymentByIdAsync(partialPaymentId)
                                 ?? throw new ArgumentException("No Partial payment found with the specified id", nameof(partialPaymentId));
            var product = await _productService.GetProductByIdAsync(productId)
                          ?? throw new ArgumentException("No product found with the specified product id", nameof(productId));
            var partialPaymentProductMapping = await _partialPaymentService.GetPartialPaymentAppliedToProductAsync(productId, partialPaymentId);
            await _partialPaymentService.DeletePartialPaymentProductMappingAsync(partialPaymentProductMapping);
            return new NullJsonResult();
        }
        [HttpPost]
        public async Task<IActionResult> ProductList(PartialPaymentProductSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePartialPayments))
                return await AccessDeniedDataTablesJson();
            //try to get a partialpayment with the specified id
            var partialPayment = await _partialPaymentService.GetPartialPaymentByIdAsync(searchModel.PartialPaymentId)
                                 ?? throw new ArgumentException("No partialPayment found with the specified id");

            //prepare model
            var model = await _partialPaymentModelFactory.PreparePartialPaymentProductListModelAsync(searchModel, partialPayment);

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!(await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePartialPayments)))
                return AccessDeniedView();

            var partialPayment = await _partialPaymentService.GetPartialPaymentByIdAsync(id);

            await _partialPaymentService.DeletePartialPaymentAsync(partialPayment);

            _notificationService.SuccessNotification(await _localizationService
                .GetResourceAsync("Admin.Promotions.PartialPayments.Deleted"));

            return RedirectToAction("List");
        }
        
        

        public async Task<IActionResult> ProductAddPopup(int partialPaymentId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePartialPayments))
                return await AccessDeniedDataTablesJson();
            //  todo : permissions
            //prepare model
            var model =
                await _partialPaymentModelFactory.PrepareAddProductToPartialPaymentSearchModelAsync(
                    new AddProductToPartialPaymentSearchModel());

            return View("~/Plugins/Payments.CustomerWallet/Views/ProductAddPopup.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAddPopupList(AddProductToPartialPaymentSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePartialPayments))
                return await AccessDeniedDataTablesJson();
            //prepare model
            var model = await _partialPaymentModelFactory.PrepareAddProductToPartialPaymentListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> ProductAddPopup(AddProductToPartialPaymentModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePartialPayments))
                return await AccessDeniedDataTablesJson();

            //try to get a partial payment with the specified id
            var partialPayment = await _partialPaymentService.GetPartialPaymentByIdAsync(model.PartialPaymentId)
                                 ?? throw new ArgumentException("No partial payment found with the specified id");

            var selectedProducts =
                await _partialPaymentService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());
            if (selectedProducts.Any())
            {
                foreach (var product in selectedProducts)
                {
                    if (await _partialPaymentService.GetPartialPaymentMappingByProductId(product.Id) is null)
                    {
                        await _partialPaymentService.InsertPartialPaymentProductMappingAsync(
                            new PartialPaymentProductMapping() {ProductId = product.Id, PartialPaymentId = partialPayment.Id});
                    }
                    else
                    {
                        _notificationService.ErrorNotification(
                            await _localizationService.GetResourceAsync("Admin.Promotions.PartialPayments.DuplicateProductError"));
                    }
                    
                }
            }

            ViewBag.RefreshPage = true;

            return View("~/Plugins/Payments.CustomerWallet/Views/ProductAddPopup.cshtml", new AddProductToPartialPaymentSearchModel());
        }
    }
}