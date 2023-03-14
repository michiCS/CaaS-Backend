using CaaS.Core.Logic.Discount.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Tests.DiscountRuleTests;

public class ProductQuantityRuleTests
{
    [Theory]
    [InlineData(1, 10)]
    [InlineData(5, 25)]
    [InlineData(10, 200)]
    public void CanApply_ReturnsTrue(int productId, int quantity)
    {
        var productQuantityRule = new ProductQuantityRule(0, 0, 0, productId, 10);
        var result = productQuantityRule.CanApply(productId, quantity);
        Assert.True(result);
    }

    [Theory]
    [InlineData(1, 10, 2, 9)]
    [InlineData(5, 20, 4, 20)]
    [InlineData(10, 50, 10, 49)]
    public void CanApply_ReturnsFalse(int ruleProductId, int ruleQuantity, int applyProductId, int applyQuantity)
    {
        var productRule = new ProductQuantityRule(0, 0, 0, ruleProductId, ruleQuantity);
        var result = productRule.CanApply(applyProductId, applyQuantity);
        Assert.False(result);
    }
}
