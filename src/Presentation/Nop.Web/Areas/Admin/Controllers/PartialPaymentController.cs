using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Helpers;
using Nop.Web.Areas.Admin.Models.PartialPayments;
using Nop.Web.Factories.PartialPayments;
using Task = Nop.Services.Tasks.Task;

namespace Nop.Web.Areas.Admin.Controllers
{
    public class PartialPaymentController : BaseAdminController
    {
        private readonly IPartialPaymentModelFactory _partialPaymentModelFactory;

        public PartialPaymentController(IPartialPaymentModelFactory partialPaymentModelFactory)
        {
            _partialPaymentModelFactory = partialPaymentModelFactory;
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
    }
}