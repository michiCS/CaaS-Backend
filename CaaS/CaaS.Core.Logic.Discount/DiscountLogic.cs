using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;
using CaaS.Core.Logic.Discount.Action;
using CaaS.Core.Logic.Discount.Rule;
using Microsoft.IdentityModel.Tokens;

namespace CaaS.Core.Logic.Discount;

public class DiscountLogic : IDiscountLogic
{
    private readonly IProductDao productDao;
    private readonly IDiscountActionDataDao discountActionDataDao;
    private readonly IDiscountRuleDataDao discountRuleDataDao;
    private readonly IOrderProductDao orderProductDao;
    private readonly ICartProductDao cartProductDao;

    public DiscountLogic(IDaoProvider daoProvider)
    {
        productDao = daoProvider.ProductDao;
        discountActionDataDao = daoProvider.DiscountActionDataDao;
        discountRuleDataDao = daoProvider.DiscountRuleDataDao;
        orderProductDao = daoProvider.OrderProductDao;
        cartProductDao = daoProvider.CartProductDao;
    }

    public async Task<decimal> GetDiscountForCartProductAsync(CartProduct cartProduct, IEnumerable<DiscountRule> rules)
    {
        var product = await productDao.FindByIdAsync(cartProduct.ProductId);
        if (product is null)
        {
            return 0;
        }

        decimal discount = 0;
        rules.ToList().ForEach(async rule =>
        {
            if (DiscountRuleHelper.CanApplyCartProductRule(cartProduct, rule))
            {
                discount = Math.Max((await GetPriceReductionAsync(product.Price, rule.ActionId)), discount);
            }
        });

        return discount;
    }

    public async Task<ProductPriceInfo> GetProductPriceInfoAsync(CartProduct cartProduct, IEnumerable<DiscountRule> rules)
    {
        var product = await productDao.FindByIdAsync(cartProduct.ProductId);
        if (product is null)
        {
            throw new ArgumentException(nameof(product));
        }

        var discount = await GetDiscountForCartProductAsync(cartProduct, rules);

        return new ProductPriceInfo
        {
            ListPrice = product.Price,
            Discount = discount,
            Price = Math.Max(product.Price - discount, 0),
            Quantity = cartProduct.Quantity
        };
    }

    public PriceInfo GetPriceInfo(IEnumerable<ProductPriceInfo> priceInfos, IEnumerable<DiscountRule> rules)
    {
        var sumDiscounts = CalculateSumDiscounts(priceInfos, rules);
        return CalculatePriceInfo(priceInfos, sumDiscounts);
    }

    public decimal CalculateSumDiscounts(IEnumerable<ProductPriceInfo> priceInfos, IEnumerable<DiscountRule> rules)
    {
        decimal totalPrice = 0;
        decimal sumDiscounts = 0;

        priceInfos.ToList().ForEach(priceInfo =>
        {
            totalPrice += priceInfo.Price * priceInfo.Quantity;
            sumDiscounts += priceInfo.Discount * priceInfo.Quantity;
        });

        var maxDiscount = FindDiscountForTotalPrice(rules, totalPrice);

        return sumDiscounts + maxDiscount;
    }

    public async Task<PriceInfo> GetPriceForOrderAsync(Order order)
    {
        decimal total = 0;
        decimal productDiscounts = 0;

        (await orderProductDao.FindByOrderIdAsync(order.Id)).ToList().ForEach(o =>
        {
            total += o.Price * o.Quantity;
            productDiscounts += o.Discount * o.Quantity;
        });

        return new PriceInfo
        {
            ListPrice = total + productDiscounts,
            CartDiscount = order.SumDiscounts - productDiscounts,
            ProductDiscount = productDiscounts,
            Total = total - (order.SumDiscounts - productDiscounts),
            SumDiscounts = order.SumDiscounts
        };
    }

    private decimal FindDiscountForTotalPrice(IEnumerable<DiscountRule> rules, decimal total)
    {
        decimal discount = 0;
        rules.ToList().ForEach(async rule =>
        {
            if (DiscountRuleHelper.CanApplyCartRule(rule, total))
            {
                discount = Math.Max((await GetPriceReductionAsync(total, rule.ActionId)), discount);
            }
        });

        return discount;
    }

    private async Task<decimal> GetPriceReductionAsync(decimal price, int actionId)
    {
        var actionData = await discountActionDataDao.FindByIdAsync(actionId);
        if (actionData is not null)
        {
            var action = DiscountActionHelper.CreateDiscountAction(actionData);
            return action?.GetPriceReduction(price) ?? 0;
        }

        return 0;
    }

    private PriceInfo CalculatePriceInfo(IEnumerable<ProductPriceInfo> priceInfos, decimal sumDiscounts)
    {
        decimal listPrice = 0;
        decimal total = 0;
        decimal productDiscounts = 0;

        priceInfos.ToList().ForEach(p =>
        {
            listPrice += p.ListPrice * p.Quantity;
            total += p.Price * p.Quantity;
            productDiscounts += p.Discount * p.Quantity;
        });

        return new PriceInfo
        {
            ListPrice = listPrice,
            CartDiscount = sumDiscounts - productDiscounts,
            ProductDiscount = productDiscounts,
            Total = total - (sumDiscounts - productDiscounts),
            SumDiscounts = sumDiscounts
        };
    }
}
