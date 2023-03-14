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

public class AdoOrderDao : IOrderDao
{
    private readonly AdoTemplate template;

    private Order MapRowToOrder(IDataRecord row) =>
        new(
                id: (int)row["id"],
                date: (DateTime)row["date"],
                cartId: (int)row["cart_id"],
                sumDiscounts: (decimal)row["sum_discounts"],
                customerId: (int)row["customer_id"],
                tenantId: (int)row["tenant_id"]
            );
    public AdoOrderDao(IConnectionFactory connectionFactory)
    {
        this.template = new AdoTemplate(connectionFactory);
    }

    public async Task<IEnumerable<Order>> FindAllAsync()
    {
        return await template.QueryAsync("select * from `order`", MapRowToOrder);
    }

    public async Task<Order?> FindByIdAsync(int id)
    {
        return await template.QuerySingleAsync($"select * from `order` where id=@id", MapRowToOrder, new QueryParameter("@id", id));
    }

    public async Task<IEnumerable<Order>> FindByTenantIdAsync(int id)
    {
        return await template.QueryAsync("select * from `order` where tenant_id = @tid", MapRowToOrder, new QueryParameter("@tid", id));
    }

    public async Task<bool> InsertAsync(Order order)
    {
        return await template.ExecuteAsync(@"insert into `order` (date, cart_id, sum_discounts, customer_id, tenant_id) values (@date, @cid, @sumdisc, @cuid, @tid)",
           new QueryParameter("@date", order.Date),
           new QueryParameter("@cid", order.CartId),
           new QueryParameter("@sumdisc", order.SumDiscounts),
           new QueryParameter("@cuid", order.CustomerId),
           new QueryParameter("@tid", order.TenantId)) == 1;
    }

    public async Task<Order?> InsertAndGetAsync(Order order)
    {
        var id = await template.ExecuteScalarAsync(@"insert into `order` (date, cart_id, sum_discounts, customer_id, tenant_id) values (@date, @cid, @sumdisc, @cuid, @tid); select last_insert_id();",
           new QueryParameter("@date", order.Date),
           new QueryParameter("@cid", order.CartId),
           new QueryParameter("@sumdisc", order.SumDiscounts),
           new QueryParameter("@cuid", order.CustomerId),
           new QueryParameter("@tid", order.TenantId));

        return id is not null ? await FindByIdAsync(Convert.ToInt32(id)) : null;
    }

    public async Task<bool> UpdateSumDiscounts(int id, decimal sumDiscounts)
    {
        return await template.ExecuteAsync(@"update `order` set sum_discounts=@sumdisc where id=@id",
            new QueryParameter("@id", id),
            new QueryParameter("@sumdisc", sumDiscounts)) == 1;
    }

    public async Task<Order?> FindByCartIdAsync(int id)
    {
        return await template.QuerySingleAsync($"select * from `order` where cart_id=@id", MapRowToOrder, new QueryParameter("@id", id));
    }
}