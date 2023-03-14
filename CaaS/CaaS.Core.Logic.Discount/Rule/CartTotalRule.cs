using CaaS.Core.Dal.Domain;
using System.Text.Json;

namespace CaaS.Core.Logic.Discount.Rule;

public class CartTotalRule : DiscountRule
{
    public decimal MinCartTotal { get; set; }

    public CartTotalRule(int id, int tenantId, int actionId, decimal minCartTotal)
        : base(id, DiscountApplicationType.Cart, tenantId, actionId)
    {
        MinCartTotal = minCartTotal;
    }

    public CartTotalRule(DiscountRuleData data) : base(data)
    {
        if (!data.MinCartTotal.HasValue)
        {
            throw new ArgumentNullException(nameof(data.MinCartTotal));
        }
        MinCartTotal = data.MinCartTotal.Value;
    }

    public bool CanApply(decimal cartTotal)
    {
        if (cartTotal < MinCartTotal)
        {
            return false;
        }
        return CanApply();
    }

    public static bool CanCreate(DiscountRuleData data)
    {
        return data.MinCartTotal is not null 
            && data.ApplicationType == DiscountApplicationType.Cart;
    }

    public override DiscountRuleData ToDiscountRuleData()
    {
        var data = base.ToDiscountRuleData();
        data.MinCartTotal = MinCartTotal;
        return data;
    }

    public override string ToDescription()
    {
        return $"Apply Discount when Cart Total >= {MinCartTotal}";
    }

    public override string ToString()
    {
        return this.GetType().Name + JsonSerializer.Serialize(this);
       
    }

}

