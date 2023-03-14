using CaaS.Core.Dal.Domain;
using CaaS.Core.Logic.Discount;
using CaaS.Core.Logic.Discount.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Tests.DiscountRuleTests;

public class DiscountRuleCreationTests
{
    public static readonly object[][] TestData =
    {
        new object[]
        {
            new DiscountRuleData {Id = 1, ActionId = 1, TenantId = 1, ApplicationType = DiscountApplicationType.Cart, MinCartTotal = 100},
            new CartTotalRule(1, 1, 1, 100)
        },
        new object[]
        {
            new DiscountRuleData {Id = 2, ActionId = 2, TenantId = 2, ApplicationType = DiscountApplicationType.Cart, DateFrom = DateTime.UtcNow.AddDays(-2), DateTo = DateTime.UtcNow.AddDays(2)},
            new TemporalRule(2, DiscountApplicationType.Cart, 2, 2, DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(2))
        },
        new object[]
        {
            new DiscountRuleData {Id = 3, ActionId = 3, TenantId = 3, ApplicationType = DiscountApplicationType.CartProduct, ProductId = 3},
            new ProductRule(3, 3, 3, 5)
        },
        new object[]
        {
            new DiscountRuleData {Id = 4, ActionId = 4, TenantId = 4, ApplicationType = DiscountApplicationType.CartProduct, ProductId = 4, MinQuantity = 4},
            new ProductQuantityRule(4, 4, 4, 4, 4)
        },
    };

    [Theory, MemberData(nameof(TestData))]
    public void CreateDiscountRule_Returns_ExpectedDiscountRule(DiscountRuleData data, DiscountRule expected)
    {
        var rule = DiscountRuleHelper.CreateDiscountRule(data);
        Assert.Equal(expected.GetType(), rule.GetType());
        Assert.Equal(expected.Id, rule.Id);
        Assert.Equal(expected.ActionId, rule.ActionId);
        Assert.Equal(expected.TenantId, rule.TenantId);
    }

    public static readonly object[][] TestData2 =
{
        new object[]
        {
            new DiscountRuleData {Id = 1, ActionId = 1, TenantId = 1, ApplicationType = DiscountApplicationType.Cart}
        },
        new object[]
        {
            new DiscountRuleData {Id = 2, ActionId = 2, TenantId = 2, ApplicationType = DiscountApplicationType.Cart, ProductId = 1, MinQuantity = 5}
        },
        new object[]
        {
             new DiscountRuleData {Id = 3, ActionId = 3, TenantId = 3, ApplicationType = DiscountApplicationType.CartProduct}
        },
        new object[]
        {
             new DiscountRuleData {Id = 4, ActionId = 4, TenantId = 4, ApplicationType = DiscountApplicationType.CartProduct, MinCartTotal = 200}
        },
        new object[]
        {
             new DiscountRuleData {Id = 5, ActionId = 5, TenantId = 5, ApplicationType = DiscountApplicationType.Cart, DateFrom = DateTime.UtcNow}
        },
        new object[]
        {
             new DiscountRuleData {Id = 6, ActionId = 6, TenantId = 6, ApplicationType = DiscountApplicationType.CartProduct, DateTo = DateTime.UtcNow}
        }
    };

    [Theory, MemberData(nameof(TestData2))]
    public void CreateDiscountRule_Returns_Null(DiscountRuleData data)
    {
        Assert.Null(DiscountRuleHelper.CreateDiscountRule(data));
    }
}
