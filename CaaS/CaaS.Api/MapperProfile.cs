using AutoMapper;
using CaaS.Api.Dtos.CartDtos;
using CaaS.Api.Dtos.CartProductDtos;
using CaaS.Api.Dtos.CustomerDtos;
using CaaS.Api.Dtos.DiscountDtos;
using CaaS.Api.Dtos.ProductDtos;
using CaaS.Api.Dtos.StatisticsDtos;
using CaaS.Api.Dtos.TenantDtos;
using CaaS.Core.Dal.Domain;
using CaaS.Core.Logic.Discount;
using CaaS.Core.Logic.Statistics;
using static CaaS.Core.Logic.Statistics.StatisticsLogic;

namespace CaaS.Api
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CartProductDtoSimple, CartProduct>()
                .ConstructUsing(src => new CartProduct(0, src.Quantity, src.CartId, src.ProductId));
            CreateMap<CartProduct, CartProductDtoSimple>();
            CreateMap<CartProductDto, ProductPriceInfo>();

            CreateMap<Cart, CartDtoSimple>();

            CreateMap<Customer, CustomerDto>();
            CreateMap<CustomerDtoForCreation, Customer>()
                .ConstructUsing(src => new Customer(0, src.Name, src.Email, src.TenantId));


            CreateMap<Product, ProductDto>();
            CreateMap<ProductDtoForCreation, Product>()
                .ConstructUsing((src, context) => new Product(0, src.Name, src.Description, src.DownloadLink, src.ImageUrl, src.Price, src.IsDeleted, (int)context.Items["tenantId"]));
            CreateMap<ProductDto, Product>()
                .ConstructUsing((src, context) => new Product(0, src.Name, src.Description, src.DownloadLink, src.ImageUrl, src.Price, src.IsDeleted, (int)context.Items["tenantId"]));


            CreateMap<DataSample, DataSampleDto>();
            CreateMap<Tuple<int, int>, OpenClosedCartsDto>()
                .ForMember(od => od.NrOpen, opt => opt.MapFrom(t => t.Item1))
                .ForMember(od => od.NrClosed, opt => opt.MapFrom(t => t.Item2));
            CreateMap<SoldProduct, SoldProductDto>();


            CreateMap<Tenant, TenantDto>();


            CreateMap<DiscountRuleData, DiscountRuleDto>()
                .ForMember(dr => dr.DateFrom, opt => opt.MapFrom(drd => drd.DateFrom.HasValue ? drd.DateFrom.Value.ToString("yyyy-MM-dd") : null))
                .ForMember(dr => dr.DateTo, opt => opt.MapFrom(drd => drd.DateTo.HasValue ? drd.DateTo.Value.ToString("yyyy-MM-dd") : null))
                .ForMember(dr => dr.Description, opt => opt.MapFrom(drd => GetDiscountRuleDescription(drd)));
            CreateMap<DiscountRuleDtoForCreation, DiscountRuleData>()
                .ForMember(dr => dr.TenantId, opt => opt.MapFrom((src, dst, _, context) => context.Items["tenantId"]));
            CreateMap<DiscountRuleDtoForUpdate, DiscountRuleData>();


            CreateMap<DiscountActionData, DiscountActionDto>()
                .ForMember(da => da.Description, opt => opt.MapFrom(dad => GetDiscountActionDescription(dad)));
            CreateMap<DiscountActionDtoForUpdate, DiscountActionData>()
                .ForMember(da => da.TenantId, opt => opt.MapFrom((src, dst, _, context) => context.Items["tenantId"]));
            CreateMap<DiscountActionDtoForCreation, DiscountActionData>()
                .ForMember(da => da.TenantId, opt => opt.MapFrom((src, dst, _, context) => context.Items["tenantId"]));
        }

        private string GetDiscountRuleDescription(DiscountRuleData discountRuleData)
        {
            return DiscountRuleHelper.CreateDiscountRule(discountRuleData)?.ToDescription() ?? "Invalid discount rule";
        }

        private string GetDiscountActionDescription(DiscountActionData discountActionData)
        {
            return DiscountActionHelper.CreateDiscountAction(discountActionData)!.ToDescription() ?? "Invalid discount action";
        }
    }

}

