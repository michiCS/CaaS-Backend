using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;
using Dal.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Dal.Ado;

public class AdoCartDao : ICartDao
{
    private readonly AdoTemplate template;

    private Cart MapRowToCart(IDataRecord row) =>
        new(
                id: (int)row["id"],
                createdOn: (DateTime)row["created_on"],
                tenantId: (int)row["tenant_id"]
            );

    public AdoCartDao(IConnectionFactory connectionFactory)
    {
        this.template = new AdoTemplate(connectionFactory);
    }

    public async Task<IEnumerable<Cart>> FindAllAsync()
    {
        return await template.QueryAsync("select * from cart", MapRowToCart);
    }

    public async Task<Cart?> FindByIdAsync(int id)
    {
        return await template.QuerySingleAsync($"select * from cart where id=@id", MapRowToCart, new QueryParameter("@id", id));
    }

    public async Task<bool> InsertAsync(Cart cart)
    {
        return await template.ExecuteAsync(@"insert into cart (tenant_id, created_on) values (@tid, @created)",
           new QueryParameter("@created", cart.CreatedOn),
           new QueryParameter("@tid", cart.TenantId)) == 1;
    }

    public async Task<Cart?> InsertAndGetAsync(Cart cart)
    {
        var id = await template.ExecuteScalarAsync(@"insert into cart (tenant_id, created_on) values (@tid, @created); select last_insert_id()",
            new QueryParameter("@created", cart.CreatedOn),
            new QueryParameter("@tid", cart.TenantId));

        return id is not null ? await FindByIdAsync(Convert.ToInt32(id)) : null;
    }

    public async Task<IEnumerable<Cart>> FindByTenantIdAsync(int id)
    {
        return await template.QueryAsync($"select * from cart where tenant_id=@id", MapRowToCart, new QueryParameter("@id", id));
    }

    public async Task<int> FindRandomId()
    {
        return await template.QuerySingleAsync($"select id from cart ORDER BY RAND() LIMIT 1",
           (IDataRecord row) => (int)row["id"]);
    }
}
