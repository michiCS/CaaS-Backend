using CaaS.Core.Dal.Domain;
using System.Text.Json;

namespace CaaS.Core.Logic.Discount.Rule;

public class ProductRule : DiscountRule
{
    public int ProductId { get; set; }

    public ProductRule(int id, int tenantId, int actionId, int productId)
        : base(id, DiscountApplicationType.CartProduct, tenantId, actionId)
    {
        ProductId = productId;
    }

    public ProductRule(DiscountRuleData data) : base(data)
    {
        if (!data.ProductId.HasValue)
        {
            throw new ArgumentException(nameof(data.ProductId));
        }
        ProductId = data.ProductId.Value;
    }

    public bool CanApply(int productId)
    {
        if (productId != ProductId)
        {
            return false;
        }

        return CanApply();
    }

    public static bool CanCreate(DiscountRuleData data)
    {
        return data.ProductId is not null 
            && data.ApplicationType == DiscountApplicationType.CartProduct;
    }

    public override DiscountRuleData ToDiscountRuleData()
    {
        var data = base.ToDiscountRuleData();
        data.ProductId = ProductId;
        return data;
    }

    public override string ToDescription()
    {
        return $"Apply Discount when Product with ID={ProductId} is ordered";
    }

    public override string ToString()
    {
        return this.GetType().Name + JsonSerializer.Serialize(this);
    }
}

