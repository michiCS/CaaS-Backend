using AutoMapper;
using CaaS.Api.Dtos.CartDtos;
using CaaS.Api.Dtos.CartProductDtos;
using CaaS.Api.Dtos.OrderDtos;
using CaaS.Api.Dtos.OrderProductDtos;
using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;
using CaaS.Core.Logic.Discount;
using CaaS.Core.Logic.Discount.Rule;

namespace CaaS.Api.Dtos;

public class DtoCreator
{
    private IDiscountRuleDataDao discountRuleDataDao;
    private ICartProductDao cartProductDao;
    private IProductDao productDao;
    private IOrderProductDao orderProductDao;
    private ICustomerDao customerDao;
    private IDiscountLogic discountLogic;
    private IMapper mapper;

    public DtoCreator(IDaoProvider daoProvider, IDiscountLogic discountLogic, IMapper mapper)
    {
        discountRuleDataDao = daoProvider.DiscountRuleDataDao;
        cartProductDao = daoProvider.CartProductDao;
        productDao = daoProvider.ProductDao;
        orderProductDao = daoProvider.OrderProductDao;
        customerDao = daoProvider.CustomerDao;
        this.discountLogic = discountLogic;
        this.mapper = mapper;
    }

    public async Task<CartDto> CreateCartDto(Cart cart)
    {
        var rules = (await discountRuleDataDao.FindByTenantIdAsync(cart.TenantId))
                        .Select(dr => DiscountRuleHelper.CreateDiscountRule(dr))
                        .Where(dr => dr is not null)
                        .Select(dr => dr!);

        var productRules = rules.Where(r => r.ApplicationType == DiscountApplicationType.CartProduct);
        var cartRules = rules.Where(r => r.ApplicationType == DiscountApplicationType.Cart);

        var cartProductDtos = (await cartProductDao.FindByCartIdAsync(cart.Id))
                                .Select(c => CreateCartProductDto(c, productRules))
                                .Select(c => c.Result);

        var priceInfo = discountLogic.GetPriceInfo(cartProductDtos.Select(c => mapper.Map<ProductPriceInfo>(c)), rules);

        return new CartDto()
        {
            Id = cart.Id,
            CreatedOn = cart.CreatedOn,
            TenantId = cart.TenantId,
            ListPrice = priceInfo.ListPrice,
            Total = priceInfo.Total,
            ProductDiscount = priceInfo.ProductDiscount,
            CartDiscount = priceInfo.CartDiscount,
            SumDiscounts = priceInfo.SumDiscounts,
            CartProducts = cartProductDtos
        };
    }

    public async Task<CartProductDto> CreateCartProductDto(CartProduct cartProduct, IEnumerable<DiscountRule> rules)
    {
        var product = await productDao.FindByIdAsync(cartProduct.ProductId);
        if (product == null)
        {
            throw new ArgumentException(nameof(product));
        }

        var priceInfo = await discountLogic.GetProductPriceInfoAsync(cartProduct, rules);

        return new CartProductDto
        {
            Id = cartProduct.Id,
            Discount = priceInfo.Discount,
            ListPrice = priceInfo.ListPrice,
            Price = priceInfo.Price,
            ProductName = product.Name,
            ProductImageUrl = product.ImageUrl,
            Quantity = priceInfo.Quantity
        };
    }

    public async Task<OrderDto> CreateOrderDto(Order order)
    {
        var orderProducts = await orderProductDao.FindByOrderIdAsync(order.Id);
        var customer = await customerDao.FindByIdAsync(order.CustomerId);

        if (customer is null)
        {
            throw new ArgumentException(nameof(customer));
        }

        var orderProductDtos = orderProducts.Select(async o => await CreateOrderProductDto(o))
                                            .Select(o => o.Result);

        var priceInfo = await discountLogic.GetPriceForOrderAsync(order);

        return new OrderDto()
        {
            Id = order.Id,
            Date = order.Date,
            ProductDiscount = priceInfo.ProductDiscount,
            CartDiscount = priceInfo.CartDiscount,
            Total = priceInfo.Total,
            ListPrice = priceInfo.ListPrice,
            SumDiscounts = priceInfo.SumDiscounts,
            CustomerEmail = customer.Email,
            CustomerName = customer.Name,
            OrderProducts = orderProductDtos
        };
    }

    public async Task<OrderProductDto> CreateOrderProductDto(OrderProduct orderProduct)
    {
        var product = await productDao.FindByIdAsync(orderProduct.ProductId);

        if (product is null)
        {
            throw new ArgumentException(nameof(product));
        }

        return new OrderProductDto()
        {
            Discount = orderProduct.Discount,
            Price = orderProduct.Price,
            Quantity = orderProduct.Quantity,
            ProductDownloadLink = product.DownloadLink,
            ProductName = product.Name,
            ProductImageUrl = product.ImageUrl
        };
    }

}
