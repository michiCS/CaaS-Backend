using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;
using Dal.Common;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Dal.Ado;

public class AdoOrderProductDao : IOrderProductDao
{
    private readonly AdoTemplate template;

    private OrderProduct MapRowToOrderProduct(IDataRecord row) =>
        new(
                id: (int)row["id"],
                productId: (int)row["product_id"],
                orderId: (int)row["order_id"],
                price: (decimal)row["price"],
                discount: (decimal)row["discount"],
                quantity: (int)row["quantity"]
            );

    public AdoOrderProductDao(IConnectionFactory connectionFactory)
    {
        this.template = new AdoTemplate(connectionFactory);
    }
    public async Task<IEnumerable<OrderProduct>> FindAllAsync()
    {
        return await template.QueryAsync("select * from order_product", MapRowToOrderProduct);
    }

    public async Task<OrderProduct?> FindByIdAsync(int id)
    {
        return await template.QuerySingleAsync($"select * from order_product where id=@id", MapRowToOrderProduct, new QueryParameter("@id", id));
    }

    public async Task<IEnumerable<OrderProduct>> FindByOrderIdAsync(int id)
    {
        return await template.QueryAsync($"select * from order_product where order_id=@id", MapRowToOrderProduct, new QueryParameter("@id", id));
    }

    public async Task<bool> InsertAsync(OrderProduct orderProduct)
    {
        return await template.ExecuteAsync(@"insert into order_product (product_id, order_id, price, discount, quantity) values (@pid, @oid, @price, @disc, @quant)",
           new QueryParameter("@pid", orderProduct.ProductId),
           new QueryParameter("@oid", orderProduct.OrderId),
           new QueryParameter("@price", orderProduct.Price),
           new QueryParameter("@disc", orderProduct.Discount),
           new QueryParameter("@quant", orderProduct.Quantity)) == 1;
    }
}
