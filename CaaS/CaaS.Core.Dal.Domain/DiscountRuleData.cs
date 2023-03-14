namespace CaaS.Core.Dal.Domain;

public enum DiscountApplicationType
{
    Cart = 0,
    CartProduct = 1
}

public class DiscountRuleData
{
    public int Id { get; set; }
    public DiscountApplicationType ApplicationType { get; set; }
    public int TenantId { get; set; }
    public decimal? MinCartTotal { get; set; }
    public int? ProductId { get; set; }
    public int? MinQuantity { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int ActionId { get; set; }
}

