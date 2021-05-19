using AutoMapper;
using Leo.Plugin.Payments.CustomerWallet.Models;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure.Mapper;

namespace Leo.Plugin.Payments.CustomerWallet.Infrastructure
{
    public class PartialPaymentMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public PartialPaymentMapperConfiguration()
        {
            CreatePartialPaymentMaps();
        }
        protected virtual void CreatePartialPaymentMaps()
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