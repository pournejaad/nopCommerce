using System;
using System.Linq;
using System.Threading.Tasks;
using Leo.Core.Payments;
using Leo.Service;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.PartialPayments;
using Nop.Web.Factories.PartialPayments;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
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

        public async Task<IActionResult> List()
        {
            // check permission on managing PartialPayment
            // TODO: add a new permission

            var model =
                await _partialPaymentModelFactory
                    .PreparePartialPaymentSearchModelAsync(new PartialPaymentSearchModel());
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> List(PartialPaymentSearchModel searchModel)
        {
            // handle permissions

            var model = await _partialPaymentModelFactory.PreparePartialPaymentListModelAsync(searchModel);
            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //handle permission
            var model = await _partialPaymentModelFactory.PreparePartialPaymentModelAsync(new PartialPaymentModel(),
                null);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(PartialPaymentModel model, bool continueEditing)
        {
            //todo: permissions
            
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
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //handle permissions
            //try to get a discount with the specified id
            var partialPayment = await _partialPaymentService.GetPartialPaymentByIdAsync(id);
            if (partialPayment == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _partialPaymentModelFactory.PreparePartialPaymentModelAsync(null, partialPayment);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(PartialPaymentModel model, bool continueEditing)
        {
            //todo: permissions
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

                return RedirectToAction("Edit", new { id = partialPayment.Id });
            }

            //prepare model
            model = await _partialPaymentModelFactory.PreparePartialPaymentModelAsync(model, partialPayment, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductList(PartialPaymentProductSearchModel searchModel)
        {
            
//todo: handle permissions

            //try to get a partialpayment with the specified id
            var partialPayment = await _partialPaymentService.GetPartialPaymentByIdAsync(searchModel.PartialPaymentId)
                           ?? throw new ArgumentException("No partialPayment found with the specified id");

            //prepare model
            PartialPaymentProductListModel model = await _partialPaymentModelFactory.PreparePartialPaymentProductListModelAsync(searchModel, partialPayment);

            return Json(model);
        }
        
        
        public async Task<IActionResult> ProductAddPopup(int partialPaymentId)
        {
        //  todo : permissions
            //prepare model
            var model =
                await _partialPaymentModelFactory.PrepareAddProductToPartialPaymentSearchModelAsync(
                    new AddProductToPartialPaymentSearchModel());

            return View(model);
        }
        [HttpPost]
        public virtual async Task<IActionResult> ProductAddPopupList(AddProductToPartialPaymentSearchModel searchModel)
        {
            //todo: handle permissions
            
            //prepare model
            var model = await _partialPaymentModelFactory.PrepareAddProductToPartialPaymentListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> ProductAddPopup(AddProductToPartialPaymentModel model)
        {
            //handle permissions
            
            //try to get a partial payment with the specified id
            var partialPayment = await _partialPaymentService.GetPartialPaymentByIdAsync(model.PartialPaymentId)
                                 ?? throw new ArgumentException("No partial payment found with the specified id");

            var selectedProducts =
                await _partialPaymentService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());
            if (selectedProducts.Any())
            {
                foreach (var product in selectedProducts)
                {
                    if (await _partialPaymentService.GetPartialPaymentAppliedToProductAsync(product.Id,
                        partialPayment.Id) is null)
                        await _partialPaymentService.InsertPartialPaymentProductMappingAsync(
                            new PartialPaymentProductMapping()
                            {
                                ProductId = product.Id,
                                PartialPaymentId = partialPayment.Id
                            });

                    await _productService.UpdateProductAsync(product);
                    await _productService.UpdateHasPartialPaymentsAppliedAsync(product);
                }
            }

            ViewBag.RefreshPage = true;

            return View(new AddProductToPartialPaymentSearchModel());
        }
    }
}