using CaaS.Core.Logic.Discount.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Tests.DiscountActionTests;

public class PercentageDiscountActionsTests
{
    [Theory]
    [InlineData(100, 50, 50)]
    [InlineData(55.5, 30, 38.85)]
    [InlineData(300, 100, 0)]
    [InlineData(120.85, 33, 80.97)]
    public void GetReducedPrice(decimal price, decimal reduction, decimal expectedReducedPrice)
    {
        var percentageDiscountAction = new PercentageDiscountAction(0, reduction, 0);
        var result = percentageDiscountAction.GetReducedPrice(price);
        Assert.Equal(expectedReducedPrice, result);
    }

    [Theory]
    [InlineData(100, 50, 50)]
    [InlineData(320.33, 100, 320.33)]
    [InlineData(452.98, 27.5, 124.57)]
    [InlineData(0, 40, 0)]
    public void GetPriceReduction(decimal price, decimal reduction, decimal expectedReduction)
    {
        var percentageDiscountAction = new PercentageDiscountAction(0, reduction, 0);
        var result = percentageDiscountAction.GetPriceReduction(price);
        Assert.Equal(expectedReduction, result);
    }
}
