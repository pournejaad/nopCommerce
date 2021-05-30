using System;
using System.Linq;
using Leo.Core.Domain;
using Leo.Plugin.Wallets.PartialPaymentAdmin.Factories;
using Leo.Plugin.Wallets.PartialPaymentAdmin.Models;
using Leo.Services.PartialPayments;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Leo.Plugin.Wallets.PartialPaymentAdmin.Controllers
{
    public class PartialPaymentController : BaseAdminController
    {
        private readonly IPartialPaymentModelFactory _partialPaymentModelFactory;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPartialPaymentService _partialPaymentService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;

        public PartialPaymentController(IPartialPaymentModelFactory partialPaymentModelFactory,
            ICustomerActivityService customerActivityService, IPartialPaymentService partialPaymentService,
            ILocalizationService localizationService, INotificationService notificationService,
            IPermissionService permissionService, IProductService productService)
        {
            _partialPaymentModelFactory = partialPaymentModelFactory;
            _customerActivityService = customerActivityService;
            _partialPaymentService = partialPaymentService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _productService = productService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public IActionResult List()
        {
            if (!_permissionService.Authorize(
                PartialPaymentStandardPermissionProvider.ManagePartialPayments))
                return AccessDeniedView();

            var model = _partialPaymentModelFactory
                .PreparePartialPaymentSearchModelAsync(new PartialPaymentSearchModel());
            return View("~/Plugins/Wallets.PartialPaymentAdmin/Views/List.cshtml", model);
        }

        [HttpPost]
        public IActionResult List(PartialPaymentSearchModel searchModel)
        {
            if (!_permissionService
                .Authorize(PartialPaymentStandardPermissionProvider.ManagePartialPayments))
                return AccessDeniedDataTablesJson();

            var model = _partialPaymentModelFactory
                .PreparePartialPaymentListModelAsync(searchModel);
            return Json(model);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(PartialPaymentStandardPermissionProvider.ManagePartialPayments))
                return AccessDeniedView();

            var model = _partialPaymentModelFactory.PreparePartialPaymentModelAsync(new PartialPaymentModel(),
                null);
            return View("~/Plugins/Wallets.PartialPaymentAdmin/Views/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(PartialPaymentModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(PartialPaymentStandardPermissionProvider.ManagePartialPayments))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var partialPayment = model.ToEntity<PartialPayment>();
                _partialPaymentService.InsertPartialPaymentAsync(partialPayment);

                //activity log
                _customerActivityService.InsertActivity("AddNewPartialPayment",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewPartialPayment"),
                        partialPayment.Name), partialPayment);

                _notificationService.SuccessNotification(
                    _localizationService.GetResource("Admin.Promotions.PartialPayments.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new {id = partialPayment.Id});
            }

            //prepare model
            model = _partialPaymentModelFactory.PreparePartialPaymentModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View("~/Plugins/Wallets.PartialPaymentAdmin/Views/Create.cshtml", model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(PartialPaymentStandardPermissionProvider.ManagePartialPayments))
                return AccessDeniedView();
            //try to get a discount with the specified id
            var partialPayment = _partialPaymentService.GetPartialPaymentByIdAsync(id);
            if (partialPayment == null)
                return RedirectToAction("List");

            //prepare model
            var model = _partialPaymentModelFactory.PreparePartialPaymentModelAsync(null, partialPayment);

            return View("~/Plugins/Wallets.PartialPaymentAdmin/Views/Edit", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(PartialPaymentModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(PartialPaymentStandardPermissionProvider.ManagePartialPayments))
                return AccessDeniedView();
            //try to get a PartialPayment with the specified id
            var partialPayment = _partialPaymentService.GetPartialPaymentByIdAsync(model.Id);
            if (partialPayment == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                partialPayment = model.ToEntity(partialPayment);
                _partialPaymentService.UpdatePartialPaymentAsync(partialPayment);

                //activity log
                _customerActivityService.InsertActivity("EditPartialPayment",
                    string.Format(_localizationService.GetResource("ActivityLog.EdiPartialPayment"), partialPayment.Name), partialPayment);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Promotions.PartialPayments.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new {id = partialPayment.Id});
            }

            //prepare model
            model = _partialPaymentModelFactory.PreparePartialPaymentModelAsync(model, partialPayment, true);

            //if we got this far, something failed, redisplay form
            return View("~/Plugins/Wallets.PartialPaymentAdmin/Views/Edit", model);
        }

        [HttpPost]
        public IActionResult ProductList(PartialPaymentProductSearchModel searchModel)
        {
            if (!_permissionService.Authorize(PartialPaymentStandardPermissionProvider.ManagePartialPayments))
                return AccessDeniedDataTablesJson();
            //try to get a PartialPayment with the specified id
            var partialPayment = _partialPaymentService.GetPartialPaymentByIdAsync(searchModel.PartialPaymentId)
                                 ?? throw new ArgumentException("No partialPayment found with the specified id");

            //prepare model
            var model = _partialPaymentModelFactory
                .PreparePartialPaymentProductListModelAsync(searchModel, partialPayment);

            return Json(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!(_permissionService.Authorize(PartialPaymentStandardPermissionProvider.ManagePartialPayments)))
                return AccessDeniedView();

            var partialPayment = _partialPaymentService.GetPartialPaymentByIdAsync(id);

            _partialPaymentService.DeletePartialPaymentAsync(partialPayment);

            _notificationService.SuccessNotification("Admin.Promotions.PartialPayments.Deleted");

            return RedirectToAction("List");
        }

        public IActionResult ProductDelete(int partialPaymentId, int productId)
        {
            if (!(_permissionService.Authorize(PartialPaymentStandardPermissionProvider.ManagePartialPayments)))
                return AccessDeniedView();
            var partialPayment = _partialPaymentService.GetPartialPaymentByIdAsync(partialPaymentId)
                                 ?? throw new ArgumentException("No Partial payment found with the specified id", nameof(partialPaymentId));
            var product = _productService.GetProductById(productId)
                          ?? throw new ArgumentException("No product found with the specified product id", nameof(productId));

            var partialPaymentProductMapping = _partialPaymentService
                .GetPartialPaymentAppliedToProductAsync(product.Id, partialPayment.Id);

            _partialPaymentService.DeletePartialPaymentProductMapping(partialPaymentProductMapping);

            return new NullJsonResult();
        }

        public IActionResult ProductAddPopup(int partialPaymentId)
        {
            if (!_permissionService.Authorize(PartialPaymentStandardPermissionProvider.ManagePartialPayments))
                return AccessDeniedDataTablesJson();

            var model =
                _partialPaymentModelFactory.PrepareAddProductToPartialPaymentSearchModelAsync(
                    new AddProductToPartialPaymentSearchModel());

            return View("~/Plugins/Wallets.PartialPaymentAdmin/Views/ProductAddPopup",model);
        }

        [HttpPost]
        public virtual IActionResult ProductAddPopupList(AddProductToPartialPaymentSearchModel searchModel)
        {
            if (!_permissionService.Authorize(PartialPaymentStandardPermissionProvider.ManagePartialPayments))
                return AccessDeniedDataTablesJson();
            //prepare model
            var model = _partialPaymentModelFactory.PrepareAddProductToPartialPaymentListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult ProductAddPopup(AddProductToPartialPaymentModel model)
        {
            if (!_permissionService.Authorize(PartialPaymentStandardPermissionProvider.ManagePartialPayments))
                return AccessDeniedDataTablesJson();
            //handle permissions

            //try to get a partial payment with the specified id
            var partialPayment = _partialPaymentService.GetPartialPaymentByIdAsync(model.PartialPaymentId)
                                 ?? throw new ArgumentException("No partial payment found with the specified id");

            var selectedProducts = _partialPaymentService
                .GetProductsByIdsAsync(model.SelectedProductIds.ToArray());
            if (selectedProducts.Any())
            {
                foreach (var product in selectedProducts)
                {
                    if (_partialPaymentService.GetPartialPaymentMappingByProductId(product.Id) is null)
                    {
                        _partialPaymentService.InsertPartialPaymentProductMappingAsync(
                            new PartialPaymentProductMapping() {ProductId = product.Id, PartialPaymentId = partialPayment.Id});
                    }
                }
            }

            ViewBag.RefreshPage = true;

            return View("~/Plugins/Wallets.PartialPaymentAdmin/Views/ProductAddPopup",new AddProductToPartialPaymentSearchModel());
        }
    }
}