using CaaS.Core.Dal.Domain;

namespace CaaS.Api.Dtos.CartProductDtos;

public class CartProductDto
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public string ProductName { get; set; } = null!;
    public string ProductImageUrl { get; set; } = null!;
    public decimal ListPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal Price { get; set; }
}
