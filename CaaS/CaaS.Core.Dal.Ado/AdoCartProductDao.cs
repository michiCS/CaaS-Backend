using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;
using Dal.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Dal.Ado;

public class AdoCartProductDao : ICartProductDao
{
    private readonly AdoTemplate template;

    private CartProduct MapRowToCartProduct(IDataRecord row) =>
        new(
                id: (int)row["id"],
                quantity: (int)row["quantity"],
                cartId: (int)row["cart_id"],
                productId: (int)row["product_id"]
            );

    public AdoCartProductDao(IConnectionFactory connectionFactory)
    {
        this.template = new AdoTemplate(connectionFactory);
    }

    public async Task<IEnumerable<CartProduct>> FindAllAsync()
    {
        return await template.QueryAsync("select * from cart_product", MapRowToCartProduct);
    }

    public async Task<CartProduct?> FindByIdAsync(int id)
    {
        return await template.QuerySingleAsync($"select * from cart_product where id=@id", MapRowToCartProduct, new QueryParameter("@id", id));
    }

    public async Task<bool> InsertAsync(CartProduct cartProduct)
    {
        return await template.ExecuteAsync(@"insert into cart_product (quantity, cart_id, product_id) values (@quant, @cid, @pid)",
           new QueryParameter("@quant", cartProduct.Quantity),
           new QueryParameter("@cid", cartProduct.CartId),
           new QueryParameter("@pid", cartProduct.ProductId)) == 1;
    }

    public async Task<CartProduct?> InsertAndGetAsync(CartProduct cartProduct)
    {
        var id = await template.ExecuteScalarAsync(@"insert into cart_product (quantity, cart_id, product_id) values (@quant, @cid, @pid); select last_insert_id()",
           new QueryParameter("@quant", cartProduct.Quantity),
           new QueryParameter("@cid", cartProduct.CartId),
           new QueryParameter("@pid", cartProduct.ProductId));

        return id is not null ? await FindByIdAsync(Convert.ToInt32(id)) : null;
    }

    public async Task<bool> UpdateQuantityAsync(int id, int quantity)
    {
        return await template.ExecuteAsync(@"update cart_product set quantity=@quant where id=@id",
           new QueryParameter("@quant", quantity),
           new QueryParameter("@id", id)) == 1;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await template.ExecuteAsync(@"delete from cart_product where id=@id",
           new QueryParameter("@id", id)) == 1;
    }

    public async Task<IEnumerable<CartProduct>> FindByCartIdAsync(int id)
    {
        return await template.QueryAsync($"select * from cart_product where cart_id=@cid", MapRowToCartProduct, new QueryParameter("@cid", id));
    }

    public async Task<bool> DeleteByCartIdAsync(int id)
    {
        return await template.ExecuteAsync(@"delete from cart_product where cart_id=@cid",
           new QueryParameter("@cid", id)) != 0;
    }
}
