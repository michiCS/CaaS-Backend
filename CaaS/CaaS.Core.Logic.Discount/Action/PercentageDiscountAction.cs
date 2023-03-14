using CaaS.Core.Dal.Domain;
using System.Text.Json;

namespace CaaS.Core.Logic.Discount.Action;

public class PercentageDiscountAction : DiscountAction
{
    public PercentageDiscountAction(int id, decimal value, int tenantId) : base(id, value, tenantId) { }

    public PercentageDiscountAction(DiscountActionData data) : base(data.Id, data.Value, data.TenantId) { }

    public override decimal GetReducedPrice(decimal price)
    {
        return Math.Round(Math.Max(price - GetPriceReduction(price), 0), 2);
    }

    public override decimal GetPriceReduction(decimal price)
    {
        return Math.Min(Math.Round(price * (Value / 100), 2), price);
    }

    public override DiscountActionData ToDiscountActionData()
    {
        var data = base.ToDiscountActionData();
        data.ActionType = DiscountActionType.Percentage;
        return data;
    }

    public static bool CanCreate(DiscountActionData data)
    {
        return data.ActionType == DiscountActionType.Percentage;
    }

    public override string ToDescription()
    {
        return $"Reduction: {Value}%";
    }
    public override string ToString()
    {
        return GetType().Name + JsonSerializer.Serialize(this);
    }
}

