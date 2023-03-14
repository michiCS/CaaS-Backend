using CaaS.Core.Dal.Domain;
using CaaS.Core.Logic.Discount;
using CaaS.Core.Logic.Discount.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Tests.DiscountActionTests;

public class DiscountActionCreationTests
{
    public static readonly object[][] TestData =
    {
        new object[] 
        {
            new DiscountActionData { Id = 1, TenantId = 1, ActionType = DiscountActionType.Percentage, Value = 10 },
            new PercentageDiscountAction(1, 10, 1)
        },
        new object[]
        {
            new DiscountActionData { Id = 2, TenantId = 2, ActionType = DiscountActionType.Fixed, Value = 20 },
            new FixedDiscountAction(2, 20, 2)
        }
    };

    [Theory, MemberData(nameof(TestData))]
    public void CreateDiscountAction_Returns_ExpectedDiscountAction(DiscountActionData data, DiscountAction expectedAction)
    {
        var action = DiscountActionHelper.CreateDiscountAction(data);
        Assert.Equal(expectedAction.GetType(), action.GetType());
        Assert.Equal(expectedAction.Id, action.Id);
        Assert.Equal(expectedAction.Value, action.Value);
        Assert.Equal(expectedAction.TenantId, action.TenantId);
    }
}
