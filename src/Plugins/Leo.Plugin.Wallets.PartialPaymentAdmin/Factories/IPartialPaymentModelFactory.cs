using System;
using System.Linq;
using Leo.Core.Domain;
using Leo.Plugin.Wallets.PartialPaymentAdmin.Models;
using Leo.Services.PartialPayments;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Helpers;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models.Extensions;

namespace Leo.Plugin.Wallets.PartialPaymentAdmin.Factories
{
    public interface IPartialPaymentModelFactory
    {
        PartialPaymentSearchModel PreparePartialPaymentSearchModelAsync(PartialPaymentSearchModel model);
        PartialPaymentListModel PreparePartialPaymentListModelAsync(PartialPaymentSearchModel searchModel);

        PartialPaymentModel PreparePartialPaymentModelAsync(PartialPaymentModel partialPaymentModel,
            PartialPayment partialPayment, bool showHidden = false);

        AddProductToPartialPaymentSearchModel PrepareAddProductToPartialPaymentSearchModelAsync(
            AddProductToPartialPaymentSearchModel addSearchModel);

        PartialPaymentProductListModel PreparePartialPaymentProductListModelAsync(
            PartialPaymentProductSearchModel searchModel, PartialPayment partialPayment);

        AddProductToPartialPaymentListModel PrepareAddProductToPartialPaymentListModelAsync(AddProductToPartialPaymentSearchModel searchModel);
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

        public PartialPaymentSearchModel PreparePartialPaymentSearchModelAsync(
            PartialPaymentSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            model.SetGridPageSize();

            return model;
        }

        public PartialPaymentListModel PreparePartialPaymentListModelAsync(
            PartialPaymentSearchModel searchModel)
        {
            var startDateUtc = searchModel.SearchStartDate.HasValue
                ? (DateTime?) _dateTimeHelper.ConvertToUtcTime(searchModel.SearchStartDate.Value,
                    _dateTimeHelper.CurrentTimeZone)
                : null;
            var endDateUtc = searchModel.SearchEndDate.HasValue
                ? (DateTime?) _dateTimeHelper.ConvertToUtcTime(searchModel.SearchEndDate.Value,
                    _dateTimeHelper.CurrentTimeZone).AddDays(1)
                : null;

            var partialPayments = (_partialPaymentService
                    .GetAllPartialPaymentsAsync(
                        partialPaymentName: searchModel.SearchPartialPaymentName,
                        startDateUtc: startDateUtc,
                        endDateUtc: endDateUtc))
                .ToPagedList(searchModel);
            var model = new PartialPaymentListModel().PrepareToGrid(searchModel,
                partialPayments, () =>
                {
                    return partialPayments.Select(pp =>
                    {
                        var pPModel = pp.ToModel<PartialPaymentModel>();
                        return pPModel;
                    });
                });
            return model;
        }

        public PartialPaymentModel PreparePartialPaymentModelAsync(PartialPaymentModel model,
            PartialPayment partialPayment, bool showHidden = false)
        {
            if (partialPayment != null)
            {
                var paymentModel = partialPayment.ToModel<PartialPaymentModel>();
                if (paymentModel != null)
                {
                    model = paymentModel;
                }

                // apply rules
                PreparePartialPaymentProductSearchModel(model.PartialPaymentProductSearchModel, partialPayment);
            }

            return model;
        }

        public AddProductToPartialPaymentSearchModel PrepareAddProductToPartialPaymentSearchModelAsync(
            AddProductToPartialPaymentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available categories
            _baseAdminModelFactory.PrepareCategories(searchModel.AvailableCategories);

            //prepare available manufacturers
            _baseAdminModelFactory.PrepareManufacturers(searchModel.AvailableManufacturers);

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            //prepare available vendors
            _baseAdminModelFactory.PrepareVendors(searchModel.AvailableVendors);

            //prepare available product types
            _baseAdminModelFactory.PrepareProductTypes(searchModel.AvailableProductTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        public PartialPaymentProductListModel PreparePartialPaymentProductListModelAsync(
            PartialPaymentProductSearchModel searchModel,
            PartialPayment partialPayment)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (partialPayment == null)
                throw new ArgumentNullException(nameof(partialPayment));

            var partialPaymentProducts = _partialPaymentService
                .GetPartialPaymentMappingsByPartialPaymentId(partialPayment.Id);

            if (partialPaymentProducts == null)
                return null;

            var products = _productService
                .GetProductsByIds(partialPaymentProducts.Select(x => x.ProductId).ToArray())
                ?.Where(x => !x.Deleted)
                .ToList();
            if (products == null)
                return null;

            var pagedProducts = new PagedList<Product>(products, searchModel.Page - 1, searchModel.PageSize);

            //prepare grid model
            var model = new PartialPaymentProductListModel().PrepareToGrid(searchModel, pagedProducts, () =>
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

        public AddProductToPartialPaymentListModel PrepareAddProductToPartialPaymentListModelAsync(AddProductToPartialPaymentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get products
            var products = _productService.SearchProducts(showHidden: true,
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                productType: searchModel.SearchProductTypeId > 0 ? (ProductType?) searchModel.SearchProductTypeId : null,
                keywords: searchModel.SearchProductName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new AddProductToPartialPaymentListModel().PrepareToGrid(searchModel, products, () =>
            {
                return products.Select(product =>
                {
                    var productModel = product.ToModel<ProductModel>();
                    productModel.SeName = _urlRecordService.GetSeName(product, 0, true, false);

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