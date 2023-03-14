using CaaS.Core.Logic.Discount.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Tests.DiscountActionTests;

public class FixedDiscountActionTests
{
    [Theory]
    [InlineData(100, 20, 80)]
    [InlineData(500, 20.32, 479.68)]
    [InlineData(100, 0, 100)]
    [InlineData(100, 200, 0)]
    public void GetReducedPrice(decimal price, decimal reduction, decimal expectedReducedPrice)
    {
        var fixedDiscountAction = new FixedDiscountAction(0, reduction, 0);
        var result = fixedDiscountAction.GetReducedPrice(price);
        Assert.Equal(expectedReducedPrice, result);
    }

    [Theory]
    [InlineData(100, 20, 20)]
    [InlineData(500, 20.32, 20.32)]
    [InlineData(100, 0, 0)]
    [InlineData(100, 200, 100)]
    [InlineData(0, 10, 0)]
    [InlineData(17.95, 20, 17.95)]
    public void GetPriceReduction(decimal price, decimal reduction, decimal expectedReduction)
    {
        var fixedDiscountAction = new FixedDiscountAction(0, reduction, 0);
        var result = fixedDiscountAction.GetPriceReduction(price);
        Assert.Equal(expectedReduction, result);
    }
}
