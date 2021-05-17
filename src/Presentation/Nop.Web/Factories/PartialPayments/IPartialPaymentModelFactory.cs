using System;
using System.Linq;
using System.Threading.Tasks;
using Leo.Core.Payments;
using Leo.Service;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Helpers;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.PartialPayments;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Factories.PartialPayments
{
    public interface IPartialPaymentModelFactory
    {
        Task<PartialPaymentSearchModel> PreparePartialPaymentSearchModelAsync(PartialPaymentSearchModel model);
        Task<PartialPaymentListModel> PreparePartialPaymentListModelAsync(PartialPaymentSearchModel searchModel);

        Task<PartialPaymentModel> PreparePartialPaymentModelAsync(PartialPaymentModel partialPaymentModel,
            PartialPayment partialPayment, bool showHidden = false);

        Task<AddProductToPartialPaymentSearchModel> PrepareAddProductToPartialPaymentSearchModelAsync(
            AddProductToPartialPaymentSearchModel addSearchModel);

        Task<PartialPaymentProductListModel> PreparePartialPaymentProductListModelAsync(
            PartialPaymentProductSearchModel searchModel, PartialPayment partialPayment);

        Task<AddProductToPartialPaymentListModel> PrepareAddProductToPartialPaymentListModelAsync(AddProductToPartialPaymentSearchModel searchModel);
    }

    public class PartialPaymentModelFactory : IPartialPaymentModelFactory
    {
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPartialPaymentService _partialPaymentService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;

        public PartialPaymentModelFactory(IDateTimeHelper dateTimeHelper, IPartialPaymentService partialPaymentService,
            IBaseAdminModelFactory baseAdminModelFactory, IProductService productService, IUrlRecordService urlRecordService)
        {
            _dateTimeHelper = dateTimeHelper;
            _partialPaymentService = partialPaymentService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _productService = productService;
            _urlRecordService = urlRecordService;
        }

        public async Task<PartialPaymentSearchModel> PreparePartialPaymentSearchModelAsync(
            PartialPaymentSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            model.SetGridPageSize();

            return model;
        }

        public async Task<PartialPaymentListModel> PreparePartialPaymentListModelAsync(
            PartialPaymentSearchModel searchModel)
        {
            var startDateUtc = searchModel.SearchStartDate.HasValue
                ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchStartDate.Value,
                    await _dateTimeHelper.GetCurrentTimeZoneAsync())
                : null;
            var endDateUtc = searchModel.SearchEndDate.HasValue
                ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchEndDate.Value,
                    await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1)
                : null;

            var partialPayments = (await _partialPaymentService.GetAllPartialPaymentsAsync(showHidden: true,
                    partialPaymentName: searchModel.SearchPartialPaymentName,
                    startDateUtc: startDateUtc,
                    endDateUtc: endDateUtc))
                .ToPagedList(searchModel);
            var model = await new PartialPaymentListModel().PrepareToGridAsync(searchModel,
                partialPayments, () =>
                {
                    return partialPayments.SelectAwait(async pp =>
                    {
                        var pPModel = pp.ToModel<PartialPaymentModel>();
                        return pPModel;
                    });
                });
            return model;
        }

        public  Task<PartialPaymentModel> PreparePartialPaymentModelAsync(PartialPaymentModel model,
            PartialPayment partialPayment, bool showHidden = false)
        {
            if (partialPayment != null)
            {
                model ??= partialPayment.ToModel<PartialPaymentModel>();

                // apply rules
                PreparePartialPaymentProductSearchModel(model.PartialPaymentProductSearchModel, partialPayment);
            }

            return Task.FromResult(model);
        }

        public async Task<AddProductToPartialPaymentSearchModel> PrepareAddProductToPartialPaymentSearchModelAsync(
            AddProductToPartialPaymentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available categories
            await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

            //prepare available manufacturers
            await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

            //prepare available product types
            await _baseAdminModelFactory.PrepareProductTypesAsync(searchModel.AvailableProductTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        public async Task<PartialPaymentProductListModel> PreparePartialPaymentProductListModelAsync(
            PartialPaymentProductSearchModel searchModel,
            PartialPayment partialPayment)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (partialPayment == null)
                throw new ArgumentNullException(nameof(partialPayment));

            //get products with applied partial payment
            var products = await _productService.GetProductsWithAppliedPartialPaymentAsync(
                partialPaymentId: partialPayment.Id,
                showHidden: false,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var pps = await _partialPaymentService.GetPartialPaymentMappingsByPartialPaymentId(partialPayment.Id);
            var ps = (await _productService.GetProductsByIdsAsync(pps.Select(x => x.ProductId).ToArray())).Where(x => !x.Deleted)
                .AsQueryable();
            
            var psPaged = await ps.ToPagedListAsync(searchModel.PartialPaymentId-1,
                searchModel.PageSize);

            //prepare grid model
            var model = new PartialPaymentProductListModel().PrepareToGrid(searchModel, psPaged, () =>
            {
                //fill in model values from the entity
                return products.Select(product =>
                {
                    var partialPaymentProductModel = product.ToModel<PartialPaymentProductModel>();
                    partialPaymentProductModel.ProductId = product.Id;
                    partialPaymentProductModel.ProductName = product.Name;

                    return partialPaymentProductModel;
                });
            });

            return model;
        }

        public async Task<AddProductToPartialPaymentListModel> PrepareAddProductToPartialPaymentListModelAsync(AddProductToPartialPaymentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get products
            var products = await _productService.SearchProductsAsync(showHidden: true,
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
                keywords: searchModel.SearchProductName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new AddProductToPartialPaymentListModel().PrepareToGridAsync(searchModel, products, () =>
            {
                return products.SelectAwait(async product =>
                {
                    var productModel = product.ToModel<ProductModel>();
                    productModel.SeName = await _urlRecordService.GetSeNameAsync(product, 0, true, false);

                    return productModel;
                });
            });

            return model;
        }

        private PartialPaymentProductSearchModel PreparePartialPaymentProductSearchModel(
            PartialPaymentProductSearchModel searchModel, PartialPayment partialPayment)
        {
            if (searchModel == null)
            {
                throw new ArgumentNullException(nameof(searchModel));
            }

            if (partialPayment == null)
            {
                throw new ArgumentNullException(nameof(partialPayment));
            }

            searchModel.PartialPaymentId = partialPayment.Id;
            searchModel.SetGridPageSize();
            return searchModel;
        }
    }
}