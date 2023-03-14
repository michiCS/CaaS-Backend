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

public class AdoDiscountActionDataDao : IDiscountActionDataDao
{
    private readonly AdoTemplate template;

    private DiscountActionData MapRowToDiscountActionData(IDataRecord row)
    {
        return new DiscountActionData
        {
            Id = (int)row["id"],
            Value = (decimal)row["value"],
            TenantId = (int)row["tenant_id"],
            ActionType = (DiscountActionType)row["action_type"]
        };
    }

    public AdoDiscountActionDataDao(IConnectionFactory connectionFactory)
    {
        this.template = new AdoTemplate(connectionFactory);
    }

    public async Task<IEnumerable<DiscountActionData>> FindAllAsync()
    {
        return await template.QueryAsync("select * from discount_action", MapRowToDiscountActionData);
    }

    public async Task<DiscountActionData?> FindByIdAsync(int id)
    {
        return await template.QuerySingleAsync($"select * from discount_action where id=@id", MapRowToDiscountActionData, new QueryParameter("@id", id));
    }

    public async Task<IEnumerable<DiscountActionData>> FindByTenantIdAsync(int id)
    {
        return await template.QueryAsync($"select * from discount_action where tenant_id=@tid", MapRowToDiscountActionData, new QueryParameter("@tid", id));
    }

    public async Task<bool> InsertAsync(DiscountActionData data)
    {
        return await template.ExecuteAsync(@"insert into discount_action (value, tenant_id, action_type) values(@value, @tid, @type)",
            new QueryParameter("@value", data.Value),
            new QueryParameter("@tid", data.TenantId),
            new QueryParameter("@type", data.ActionType)) == 1;
    }

    public async Task<DiscountActionData?> InsertAndGetAsync(DiscountActionData data)
    {
        var id = await template.ExecuteScalarAsync(@"insert into discount_action (value, tenant_id, action_type) values(@value, @tid, @type); select last_insert_id()",
            new QueryParameter("@value", data.Value),
            new QueryParameter("@tid", data.TenantId),
            new QueryParameter("@type", data.ActionType));

        return id is not null ? await FindByIdAsync(Convert.ToInt32(id)) : null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await template.ExecuteAsync(@"delete from discount_action where id = @id",
            new QueryParameter("@id", id)) == 1;
    }

    public async Task<bool> UpdateAsync(DiscountActionData data)
    {
        return await template.ExecuteAsync(@"update discount_action set value=@value, action_type=@type where id=@id",
            new QueryParameter("id", data.Id),
            new QueryParameter("@value", data.Value),
            new QueryParameter("@type", data.ActionType)) == 1;
    }

}
