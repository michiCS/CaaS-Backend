using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CaaS.Core.Dal.Domain;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string DownloadLink { get; set; }
    public string ImageUrl { get; set; }
    public decimal Price { get; set; }
    public bool IsDeleted { get; set; }
    public int TenantId { get; set; }

    public Product(int id, string name, string description, string downloadLink, string imageUrl, decimal price, bool isDeleted, int tenantId)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        DownloadLink = downloadLink ?? throw new ArgumentNullException(nameof(downloadLink));
        ImageUrl = imageUrl ?? throw new ArgumentNullException(nameof(imageUrl));
        Price = price;
        IsDeleted = isDeleted;
        TenantId = tenantId;
    }

    public override string ToString()
    {
        return this.GetType().Name + JsonSerializer.Serialize(this);
    }
}
