using CaaS.Core.Logic.Discount.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Tests.DiscountRuleTests;

public class ProductRuleTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void CanApply_ReturnsTrue(int productId)
    {
        var productRule = new ProductRule(0, 0, 0, productId);
        var result = productRule.CanApply(productId);
        Assert.True(result);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(5, 0)]
    [InlineData(30, 5000)]
    public void CanApply_ReturnsFalse(int ruleProductId, int applyProductId)
    {
        var productRule = new ProductRule(0, 0, 0, ruleProductId);
        var result = productRule.CanApply(applyProductId);
        Assert.False(result);
    }
}
