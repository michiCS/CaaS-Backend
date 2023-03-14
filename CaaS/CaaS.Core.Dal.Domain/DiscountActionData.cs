namespace CaaS.Core.Dal.Domain;

public enum DiscountActionType
{
    Fixed = 0,
    Percentage = 1
}

public class DiscountActionData
{
    public int Id { get; set; }
    public decimal Value { get; set; }
    public DiscountActionType ActionType { get; set; } 
    public int TenantId { get; set; }
}