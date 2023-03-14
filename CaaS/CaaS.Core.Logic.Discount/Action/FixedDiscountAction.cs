using CaaS.Core.Dal.Domain;
using System.Text.Json;

namespace CaaS.Core.Logic.Discount.Action;

public class FixedDiscountAction : DiscountAction
{
    public FixedDiscountAction(int id, decimal value, int tenantId) : base(id, value, tenantId) { }

    public FixedDiscountAction(DiscountActionData data) : base(data.Id, data.Value, data.TenantId){ }

    public override decimal GetReducedPrice(decimal price)
    {
        return Math.Max(price - Value, 0);
    }

    public override decimal GetPriceReduction(decimal price)
    {
        return Math.Min(Value, price);
    }

    public override DiscountActionData ToDiscountActionData()
    {
        var data = base.ToDiscountActionData();
        data.ActionType = DiscountActionType.Fixed;
        return data;
    }

    public static bool CanCreate(DiscountActionData data)
    {
        return data.ActionType == DiscountActionType.Fixed;
    }

    public override string ToDescription()
    {
        return $"Reduction: {Value}$";
    }
    public override string ToString()
    {
        return GetType().Name + JsonSerializer.Serialize(this);
    }
}

