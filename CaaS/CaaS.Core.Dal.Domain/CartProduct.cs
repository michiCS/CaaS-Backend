using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CaaS.Core.Dal.Domain;

public class CartProduct
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public int CartId { get; set; }
    public int ProductId { get; set; }

    public CartProduct(int id, int quantity, int cartId, int productId)
    {
        Id = id;
        Quantity = quantity;
        CartId = cartId;
        ProductId = productId;
    }

    public override string ToString()
    {
        return this.GetType().Name + JsonSerializer.Serialize(this);
    }
}
