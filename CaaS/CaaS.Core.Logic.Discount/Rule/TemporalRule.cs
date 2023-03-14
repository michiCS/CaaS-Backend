using CaaS.Core.Dal.Domain;
using System.Text.Json;

namespace CaaS.Core.Logic.Discount.Rule;

public class TemporalRule : DiscountRule
{
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }

    public TemporalRule(int id, DiscountApplicationType applicationType, int tenantId, int actionId, DateTime dateFrom, DateTime dateTo)
        : base(id, applicationType, tenantId, actionId)
    {
        DateFrom = dateFrom;
        DateTo = dateTo;
    }

    public TemporalRule(DiscountRuleData data) : base(data)
    {
        if (!data.DateFrom.HasValue)
        {
            throw new ArgumentException(nameof(data.DateFrom));
        }
        if (!data.DateTo.HasValue)
        {
            throw new ArgumentException(nameof(data.DateTo));
        }
        DateFrom = data.DateFrom.Value;
        DateTo = data.DateTo.Value;
    }

    public bool CanApply(DateTime date)
    {
        if (date < DateFrom || date > DateTo)
        {
            return false;
        }
        return CanApply();
    }

    public static bool CanCreate(DiscountRuleData data)
    {
        return data.DateFrom is not null 
            && data.DateTo is not null;
    }

    public override DiscountRuleData ToDiscountRuleData()
    {
        var data = base.ToDiscountRuleData();
        data.DateFrom = DateFrom;
        data.DateTo = DateTo;
        return data;
    }

    public override string ToDescription()
    {
        return $"Apply Discount when ordered between {DateFrom.ToString("yyyy-MM-dd")} and {DateTo.ToString("yyyy-MM-dd")}";
    }

    public override string ToString()
    {
        return this.GetType().Name + JsonSerializer.Serialize(this);
    }
}

