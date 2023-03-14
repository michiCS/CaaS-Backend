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

public class AdoTenantDao : ITenantDao
{
    private readonly AdoTemplate template;

    private Tenant MapRowToTenant(IDataRecord row) =>
        new(
                id: (int)row["id"],
                name: (string)row["name"],
                appKey: (string)row["app_key"]
            );
    public AdoTenantDao(IConnectionFactory connectionFactory)
    {
        this.template = new AdoTemplate(connectionFactory);
    }

    public async Task<IEnumerable<Tenant>> FindAllAsync()
    {
        return await template.QueryAsync("select * from tenant", MapRowToTenant);
    }

    public async Task<Tenant?> FindByIdAsync(int id)
    {
        return await template.QuerySingleAsync($"select * from tenant where id=@id", MapRowToTenant, new QueryParameter("@id", id));
    }

    public async Task<bool> InsertAsync(Tenant tenant)
    {
        return await template.ExecuteAsync(@"insert into tenant (name, app_key) values (@name, @key)",
            new QueryParameter("@name", tenant.Name),
            new QueryParameter("@key", tenant.AppKey)) == 1;
    }

    public async Task<Tenant?> InsertAndGetAsync(Tenant tenant)
    {
        var id = await template.ExecuteScalarAsync(@"insert into tenant (name, app_key) values (@name, @key); select last_insert_id()",
            new QueryParameter("@name", tenant.Name),
            new QueryParameter("@key", tenant.AppKey));

        return id is not null ? await FindByIdAsync(Convert.ToInt32(id)) : null;
    }

    public async Task<Tenant?> FindByAppKey(string key)
    {
        return await template.QuerySingleAsync($"select * from tenant where app_key=@key", MapRowToTenant, new QueryParameter("@key", key));
    }

    public async Task<bool> UpdateAsync(Tenant tenant)
    {
        return await template.ExecuteAsync(@"update tenant set name=@name, app_key=@key where id=@id",
            new QueryParameter("@id", tenant.Id),
            new QueryParameter("@name", tenant.Name),
            new QueryParameter("@key", tenant.AppKey)) == 1;
    }
}
