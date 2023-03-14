using CaaS.Core.Dal.Domain;
using CaaS.Core.Logic.Discount.Action;
using CaaS.Core.Logic.Discount.Rule;

namespace CaaS.Core.Logic.Discount;

public static class DiscountRuleHelper
{
    internal static bool CanApplyCartProductRule(CartProduct cartProduct, DiscountRule discountRule)
    {
        if (discountRule.ApplicationType != DiscountApplicationType.CartProduct)
        {
            return false;
        }

        if (discountRule is ProductQuantityRule)
        {
            return ((ProductQuantityRule)discountRule).CanApply(cartProduct.ProductId, cartProduct.Quantity);
        }

        if (discountRule is ProductRule)
        {
            return ((ProductRule)discountRule).CanApply(cartProduct.ProductId);
        }

        if (discountRule is TemporalRule)
        {
            return ((TemporalRule)discountRule).CanApply(DateTime.UtcNow);
        }

        return false;
    }

    internal static bool CanApplyCartRule(DiscountRule discountRule, decimal total)
    {
        if (discountRule.ApplicationType != DiscountApplicationType.Cart)
        {
            return false;
        }

        if (discountRule is TemporalRule)
        {
            return ((TemporalRule)discountRule).CanApply(DateTime.UtcNow);
        }

        if (discountRule is CartTotalRule)
        {
            return ((CartTotalRule)discountRule).CanApply(total);
        }

        return false;
    }

    public static DiscountRule? CreateDiscountRule(DiscountRuleData data)
    {
        if (CartTotalRule.CanCreate(data))
        {
            return new CartTotalRule(data);
        }

        if (ProductQuantityRule.CanCreate(data))
        {
            return new ProductQuantityRule(data);
        }

        if (ProductRule.CanCreate(data))
        {
            return new ProductRule(data);
        }

        if (TemporalRule.CanCreate(data))
        {
            return new TemporalRule(data);
        }

        return null;
    }
}
