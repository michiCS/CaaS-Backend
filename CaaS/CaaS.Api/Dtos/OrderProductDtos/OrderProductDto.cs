namespace CaaS.Api.Dtos.OrderProductDtos;

public class OrderProductDto
{
    public string ProductName { get; set; } = null!;
    public string ProductDownloadLink { get; set; } = null!;
    public string ProductImageUrl { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public int Quantity { get; set; }
}
