using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CaaS.Core.Dal.Domain;

public class Order
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int CartId { get; set; }
    public decimal SumDiscounts { get; set; }
    public int CustomerId { get; set; }
    public int TenantId { get; set; }

    public Order(int id, DateTime date, int cartId, decimal sumDiscounts, int customerId, int tenantId)
    {
        Id = id;
        Date = date;    
        CartId = cartId;
        SumDiscounts = sumDiscounts;
        CustomerId = customerId;
        TenantId = tenantId;
    }

    public override string ToString()
    {
        return this.GetType().Name + JsonSerializer.Serialize(this);
    }

    public override bool Equals(object? obj)
    {
        return obj is Order order &&
               Id == order.Id &&
               Date == order.Date &&
               CartId == order.CartId &&
               SumDiscounts == order.SumDiscounts &&
               CustomerId == order.CustomerId && 
               TenantId == order.TenantId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Date, CartId, SumDiscounts, CustomerId, TenantId);
    }
}
