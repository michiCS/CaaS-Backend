using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;
using CaaS.Core.Logic.Discount;
using CaaS.Core.Logic.Discount.Rule;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Transactions;

namespace CaaS.Core.Logic.OrderProcessing;

public class OrderProcessingLogic : IOrderProcessingLogic
{
    private readonly IOrderDao orderDao;
    private readonly IOrderProductDao orderProductDao;
    private readonly ICartProductDao cartProductDao;
    private readonly IProductDao productDao;
    private readonly IDiscountRuleDataDao discountRuleDataDao;
    private readonly IDiscountLogic discountLogic;

    public OrderProcessingLogic(IDaoProvider daoProvider, IDiscountLogic discountLogic)
    {
        orderDao = daoProvider.OrderDao;
        orderProductDao = daoProvider.OrderProductDao;
        cartProductDao = daoProvider.CartProductDao;
        productDao = daoProvider.ProductDao;
        discountRuleDataDao = daoProvider.DiscountRuleDataDao;
        this.discountLogic = discountLogic;
    }

    public async Task<Order?> ProcessOrderAsync(Cart cart, Customer customer)
    {
        if (cart.TenantId != customer.TenantId
            || (await orderDao.FindByCartIdAsync(cart.Id)) is not null
            || (await cartProductDao.FindByCartIdAsync(cart.Id)).IsNullOrEmpty())
        {
            return null;
        }

        Order? result = null;

        var discountRules = (await discountRuleDataDao.FindByTenantIdAsync(customer.TenantId))
                                   .Select(dr => DiscountRuleHelper.CreateDiscountRule(dr))
                                   .Where(dr => dr is not null)
                                   .Select(dr => dr!);

        var productRules = discountRules.Where(dr => dr.ApplicationType == DiscountApplicationType.CartProduct);
        var cartRules = discountRules.Where(dr => dr.ApplicationType == DiscountApplicationType.Cart);

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var order = (await orderDao.InsertAndGetAsync(new Order(0, DateTime.UtcNow.Date, cart.Id, 0, customer.Id, customer.TenantId)));
            if (order is not null)
            {
                var orderProducts = await GetOrderProductsAsync(cart.Id, order.Id, discountRules);

                if(orderProducts.IsNullOrEmpty())
                {
                    scope.Dispose();
                    return null;
                }

                var priceInfos = orderProducts.Select(op => new ProductPriceInfo
                {
                    Discount = op.Discount,
                    Price = op.Price,
                    Quantity = op.Quantity,
                });

                var sumDiscounts = discountLogic.CalculateSumDiscounts(priceInfos, cartRules);
                await orderDao.UpdateSumDiscounts(order.Id, sumDiscounts);

                orderProducts.ToList().ForEach(async orderProduct => await orderProductDao.InsertAsync(orderProduct));
                result = await orderDao.FindByIdAsync(order.Id);
            }
            scope.Complete();
        }

        return result;
    }

    private async Task<IEnumerable<OrderProduct>> GetOrderProductsAsync(int cartId, int orderId, IEnumerable<DiscountRule> discountRules)
    {
        var orderProducts = new List<OrderProduct>();
        var cartProducts = (await cartProductDao.FindByCartIdAsync(cartId)).ToList();
        
        bool cancel = false;

        foreach(var cartProduct in cartProducts)
        {
            var orderProduct = await CreateOrderProductAsync(cartProduct, orderId, discountRules);
            if(orderProduct is null)
            {
                cancel = true;
                break;
            }
            orderProducts.Add(orderProduct);
        };

        return cancel ? new List<OrderProduct>() : orderProducts;
    }

    private async Task<OrderProduct?> CreateOrderProductAsync(CartProduct cartProduct, int orderId,  IEnumerable<DiscountRule> discountRules)
    {
        var product = await productDao.FindByIdAsync(cartProduct.ProductId);

        if (product is null || product.IsDeleted)
        {
            return null;
        }

        var discount = await discountLogic.GetDiscountForCartProductAsync(cartProduct, discountRules);
        return new OrderProduct(0, product.Id, orderId, Math.Max(product.Price - discount, 0), discount, cartProduct.Quantity);
    }
}
