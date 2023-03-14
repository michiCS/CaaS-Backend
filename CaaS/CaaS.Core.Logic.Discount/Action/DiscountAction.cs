using CaaS.Core.Dal.Domain;
using System.Data;
using System.Reflection;

namespace CaaS.Core.Logic.Discount.Action;

public abstract class DiscountAction
{
    public int Id { get; set; }
    public decimal Value { get; set; }
    public int TenantId { get; set; }

    protected DiscountAction(int id, decimal value, int tenantId)
    {
        Id = id;
        Value = value;
        TenantId = tenantId;
    }

    protected DiscountAction(DiscountActionData data) : this(data.Id, data.Value, data.TenantId) { }

    public abstract decimal GetReducedPrice(decimal price);

    public abstract decimal GetPriceReduction(decimal price);

    public abstract string ToDescription();

    public virtual DiscountActionData ToDiscountActionData()
    {
        return new DiscountActionData
        {
            Id = Id,
            Value = Value,
            TenantId = TenantId,
        };
    }
}

