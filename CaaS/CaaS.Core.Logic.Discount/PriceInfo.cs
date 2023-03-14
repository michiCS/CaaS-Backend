namespace CaaS.Core.Logic.Discount;

public class PriceInfo
{
    public decimal ListPrice { get; set; }
    public decimal CartDiscount { get; set; }
    public decimal ProductDiscount { get; set; }
    public decimal SumDiscounts { get; set; }
    public decimal Total { get; set; }
}