using CaaS.Core.Dal.Domain;
using CaaS.Core.Logic.Discount.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Tests.DiscountRuleTests;

public class TemporalRuleTests
{
    public static readonly object[][] TestData =
    {
        new object[] {DateTime.UtcNow.Date.AddDays(-5), DateTime.UtcNow.Date.AddDays(5).Date},
        new object[] {DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(5)},
        new object[] {DateTime.UtcNow.Date.AddDays(-5), DateTime.UtcNow.Date}
    };

    [Theory, MemberData(nameof(TestData))]
    public void CanApply_ReturnsTrue(DateTime from, DateTime to)
    {
        var temporalRule = new TemporalRule(0, DiscountApplicationType.Cart, 0, 0, from, to);
        var result = temporalRule.CanApply(DateTime.UtcNow.Date);
        Assert.True(result);
    }

    public static readonly object[][] TestData2 =
    {
        new object[] {DateTime.UtcNow.Date.AddDays(1), DateTime.UtcNow.Date.AddDays(2)},
        new object[] {DateTime.UtcNow.Date.AddDays(-5).Date, DateTime.UtcNow.Date.AddDays(-1)},
    };

    [Theory, MemberData(nameof(TestData2))]
    public void CanApply_ReturnsFalse(DateTime from, DateTime to)
    {
        var temporalRule = new TemporalRule(0, DiscountApplicationType.Cart, 0, 0, from, to);
        var result = temporalRule.CanApply(DateTime.UtcNow.Date);
        Assert.False(result);
    }
}
