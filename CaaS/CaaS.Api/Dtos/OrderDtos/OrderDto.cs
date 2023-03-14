using CaaS.Api.Dtos.OrderProductDtos;

namespace CaaS.Api.Dtos.OrderDtos;

public class OrderDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Total { get; set; }
    public decimal ListPrice { get; set; }
    public decimal ProductDiscount { get; set; }
    public decimal CartDiscount { get; set; }
    public decimal SumDiscounts { get; set; }
    public string CustomerEmail { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public IEnumerable<OrderProductDto> OrderProducts { get; set; } = null!;
}
