﻿namespace CaaS.Api.Dtos.ProductDtos;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string DownloadLink { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public decimal Price { get; set; }
    public bool IsDeleted { get; set; }
}
