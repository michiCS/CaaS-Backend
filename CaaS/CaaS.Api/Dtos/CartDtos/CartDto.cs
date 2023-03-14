using CaaS.Api.Dtos.CartProductDtos;

namespace CaaS.Api.Dtos.CartDtos;

public class CartDto
{
    public int Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public int TenantId { get; set; }
    public decimal ListPrice { get; set; }
    public decimal Total { get; set; }
    public decimal ProductDiscount { get; set; }
    public decimal CartDiscount { get; set; }
    public decimal SumDiscounts { get; set; }
    public IEnumerable<CartProductDto> CartProducts { get; set; } = null!;
}
