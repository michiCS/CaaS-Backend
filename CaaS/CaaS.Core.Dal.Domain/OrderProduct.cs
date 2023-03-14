using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CaaS.Core.Dal.Domain;

public class OrderProduct
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int OrderId { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public int Quantity { get; set; }

    public OrderProduct(int id, int productId, int orderId, decimal price, decimal discount, int quantity)
    {
        Id = id;
        ProductId = productId;
        OrderId = orderId;
        Price = price;
        Discount = discount;
        Quantity = quantity;
    }

    public override string ToString()
    {
        return this.GetType().Name + JsonSerializer.Serialize(this);
    }

    public override bool Equals(object? obj)
    {
        return obj is OrderProduct product &&
               Id == product.Id &&
               ProductId == product.ProductId &&
               OrderId == product.OrderId &&
               Price == product.Price &&
               Discount == product.Discount &&
               Quantity == product.Quantity;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, ProductId, OrderId, Price, Discount, Quantity);
    }
}
