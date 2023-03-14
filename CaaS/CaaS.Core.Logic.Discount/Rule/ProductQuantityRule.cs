using CaaS.Core.Dal.Domain;
using System.Text.Json;
using System.Transactions;

namespace CaaS.Core.Logic.Discount.Rule;

public class ProductQuantityRule : ProductRule
{
    public int MinQuantity { get; set; }

    public ProductQuantityRule(int id, int tenantId, int actionId, int productId, int minQuantity)
        : base(id, tenantId, actionId, productId)
    {
        MinQuantity = minQuantity;
    }

    public ProductQuantityRule(DiscountRuleData data) : base(data)
    {
        if (!data.MinQuantity.HasValue)
        {
            throw new ArgumentException(nameof(data.MinQuantity));
        }
        MinQuantity = data.MinQuantity.Value;
    }

    public bool CanApply(int productId, int quantity)
    {
        if (quantity < MinQuantity)
        {
            return false;
        }

        return CanApply(productId);
    }

    public static new bool CanCreate(DiscountRuleData data)
    {
        return data.MinQuantity is not null 
            && data.ProductId is not null 
            && data.ApplicationType == DiscountApplicationType.CartProduct;
    }

    public override DiscountRuleData ToDiscountRuleData()
    {
        var data = base.ToDiscountRuleData();
        data.MinQuantity = MinQuantity;
        return data;
    }

    public override string ToDescription()
    {
        return $"Apply Discount when Product with ID={ProductId} is ordered at least {MinQuantity} times";
    }

    public override string ToString()
    {
        return this.GetType().Name + JsonSerializer.Serialize(this);
    }
}

