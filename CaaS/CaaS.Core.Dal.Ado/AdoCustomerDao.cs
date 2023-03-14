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

public class AdoCustomerDao : ICustomerDao
{
    private readonly AdoTemplate template;

    private Customer MapRowToCustomer(IDataRecord row) =>
        new(
                id: (int)row["id"],
                name: (string)row["name"],
                email: (string)row["email"],
                tenantId: (int)row["tenant_id"]
            );
    public AdoCustomerDao(IConnectionFactory connectionFactory)
    {
        this.template = new AdoTemplate(connectionFactory);
    }

    public async Task<IEnumerable<Customer>> FindAllAsync()
    {
        return await template.QueryAsync("select * from customer", MapRowToCustomer);
    }

    public async Task<Customer?> FindByIdAsync(int id)
    {
        return await template.QuerySingleAsync($"select * from customer where id=@id", MapRowToCustomer, new QueryParameter("@id", id));
    }

    public async Task<IEnumerable<Customer>> FindByTenantIdAsync(int id)
    {
        return await template.QueryAsync($"select * from customer where tenant_id=@tid", MapRowToCustomer, new QueryParameter("@tid", id));
    }

    public async Task<Customer?> FindByEmailAndTenantIdAsync(string email, int id)
    {
        return await template.QuerySingleAsync($"select * from customer where email=@email and tenant_id=@tid", MapRowToCustomer, 
            new QueryParameter("@email", email),
            new QueryParameter("@tid", id));
    }

    public async Task<bool> InsertAsync(Customer customer)
    {
        return await template.ExecuteAsync(@"insert into customer (name, email, tenant_id) values (@name, @email, @tid)",
            new QueryParameter("@name", customer.Name),
            new QueryParameter("@email", customer.Email),
            new QueryParameter("@tid", customer.TenantId)) == 1;
    }

    public async Task<Customer?> InsertAndGetAsync(Customer customer)
    {
        var id = await template.ExecuteScalarAsync(@"insert into customer (name, email, tenant_id) values (@name, @email, @tid); select last_insert_id()",
            new QueryParameter("@name", customer.Name),
            new QueryParameter("@email", customer.Email),
            new QueryParameter("@tid", customer.TenantId));

        return id is not null ? await FindByIdAsync(Convert.ToInt32(id)) : null;
    }

    public async Task<int> FindRandomCustomerIdByTenantId(int id)
    {
        return await template.QuerySingleAsync($"select id from customer where tenant_id=@id ORDER BY RAND() LIMIT 1",
            (IDataRecord row) => (int)row["id"], new QueryParameter("@id", id));
    }

}
