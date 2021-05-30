using System.Collections.Generic;
using AutoMapper;
using Leo.Core.Domain;
using Leo.Plugin.Wallets.PartialPaymentAdmin.Models;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure.Mapper;
using Nop.Services.Themes;
using Nop.Web.Areas.Admin.Infrastructure.Mapper;

namespace Leo.Plugin.Wallets.PartialPaymentAdmin.Extensions
{
    public class WalletMapperConfiguration : Profile,IOrderedMapperProfile
    {
        public WalletMapperConfiguration():base()
        {
            CreatePartialPaymentMaps();
        }

        private void CreatePartialPaymentMaps()
        {
            CreateMap<PartialPayment, PartialPaymentModel>();
            CreateMap<PartialPaymentModel, PartialPayment>();
            CreateMap<Product, PartialPaymentProductModel>()
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.ProductName, opt => opt.Ignore());
        }

        public int Order => 1;
    }
}