using CaaS.Core.Logic.Discount.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Tests.DiscountRuleTests;

public class CartTotalRuleTests
{
    [Theory]
    [InlineData(100, 100)]
    [InlineData(50, 50.01)]
    [InlineData(500, 2000)]
    public void CanApply_ReturnsTrue(decimal minCartTotal, decimal applyCartTotal)
    {
        var cartTotalRule = new CartTotalRule(0, 0, 0, minCartTotal);
        var result = cartTotalRule.CanApply(applyCartTotal);
        Assert.True(result);
    }

    [Theory]
    [InlineData(100, 0)]
    [InlineData(500, 499.99)]
    [InlineData(40, 30)]
    public void CanApply_ReturnsFalse(decimal minCartTotal, decimal applyCartTotal)
    {
        var cartTotalRule = new CartTotalRule(0, 0, 0, minCartTotal);
        var result = cartTotalRule.CanApply(applyCartTotal);
        Assert.False(result);
    }
}
