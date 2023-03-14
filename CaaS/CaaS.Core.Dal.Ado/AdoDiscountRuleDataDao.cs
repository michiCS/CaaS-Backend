using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;
using Dal.Common;
using System.Data;

namespace CaaS.Core.Dal.Ado;

public class AdoDiscountRuleDataDao : IDiscountRuleDataDao
{
    private readonly AdoTemplate template;

    private DiscountRuleData MapRowToDiscountRuleData(IDataRecord row)
    {
        return new DiscountRuleData
        {
            Id = (int)row["id"],
            ApplicationType = (DiscountApplicationType)row["application_type"],
            MinCartTotal = row.GetValueOrDefault<decimal>("min_cart_total"),
            MinQuantity = row.GetValueOrDefault<int>("min_quantity"),
            DateFrom = row.GetValueOrDefault<DateTime>("date_from"),
            DateTo = row.GetValueOrDefault<DateTime>("date_to"),
            ActionId = (int)row["action_id"],
            TenantId = (int)row["tenant_id"],
            ProductId = row.GetValueOrDefault<int>("product_id")
        };
    }

    public AdoDiscountRuleDataDao(IConnectionFactory connectionFactory)
    {
        this.template = new AdoTemplate(connectionFactory);
    }

    public async Task<IEnumerable<DiscountRuleData>> FindAllAsync()
    {
        return await template.QueryAsync("select * from discount_rule", MapRowToDiscountRuleData);
    }

    public async Task<DiscountRuleData?> FindByIdAsync(int id)
    {
        return await template.QuerySingleAsync($"select * from discount_rule where id=@id", MapRowToDiscountRuleData, new QueryParameter("@id", id));
    }

    public async Task<DiscountRuleData?> FindByProductIdAsync(int id)
    {
        return await template.QuerySingleAsync($"select * from discount_rule where product_id=@pid", MapRowToDiscountRuleData, new QueryParameter("@pid", id));
    }

    public async Task<IEnumerable<DiscountRuleData>> FindByTenantIdAsync(int id)
    {
        return await template.QueryAsync($"select * from discount_rule where tenant_id=@tid", MapRowToDiscountRuleData, new QueryParameter("@tid", id));
    }

    public async Task<IEnumerable<DiscountRuleData>> FindByActionIdAsync(int id)
    {
        return await template.QueryAsync($"select * from discount_rule where action_id=@aid", MapRowToDiscountRuleData, new QueryParameter("@aid", id));
    }

    public async Task<bool> InsertAsync(DiscountRuleData data)
    {
        return await template.ExecuteAsync(@"insert into discount_rule (application_type, min_cart_total, min_quantity, date_from, date_to, action_id, tenant_id, product_id) values (@apptype, @mintotal, @minquant, @from, @to, @aid, @tid, @pid)",
            new QueryParameter("@apptype", data.ApplicationType),
            new QueryParameter("@mintotal", data.MinCartTotal),
            new QueryParameter("@minquant", data.MinQuantity),
            new QueryParameter("@from", data.DateFrom),
            new QueryParameter("@to", data.DateTo),
            new QueryParameter("@aid", data.ActionId),
            new QueryParameter("@tid", data.TenantId),
            new QueryParameter("@pid", data.ProductId)) == 1;
    }

    public async Task<DiscountRuleData?> InsertAndGetAsync(DiscountRuleData data)
    {
        var id = await template.ExecuteScalarAsync(@"insert into discount_rule (application_type, min_cart_total, min_quantity, date_from, date_to, action_id, tenant_id, product_id) values (@apptype, @mintotal, @minquant, @from, @to, @aid, @tid, @pid); select last_insert_id()",
            new QueryParameter("@apptype", data.ApplicationType),
            new QueryParameter("@mintotal", data.MinCartTotal),
            new QueryParameter("@minquant", data.MinQuantity),
            new QueryParameter("@from", data.DateFrom),
            new QueryParameter("@to", data.DateTo),
            new QueryParameter("@aid", data.ActionId),
            new QueryParameter("@tid", data.TenantId),
            new QueryParameter("@pid", data.ProductId));

        return id is not null ? await FindByIdAsync(Convert.ToInt32(id)) : null;
    }
            
    public async Task<bool> DeleteAsync(int id)
    {
        return await template.ExecuteAsync(@"delete from discount_rule where id = @id",
            new QueryParameter("@id", id)) == 1;
    }

    public async Task<bool> UpdateAsync(DiscountRuleData data)
    {
        return await template.ExecuteAsync(@"update discount_rule set application_type=@apptype, min_cart_total=@mintotal, min_quantity=@minquant, date_from=@from, date_to=@to, action_id=@aid, product_id=@pid where id=@id",
            new QueryParameter("id", data.Id),
            new QueryParameter("@apptype", data.ApplicationType),
            new QueryParameter("@mintotal", data.MinCartTotal),
            new QueryParameter("@minquant", data.MinQuantity),
            new QueryParameter("@from", data.DateFrom),
            new QueryParameter("@to", data.DateTo),
            new QueryParameter("@aid", data.ActionId),
            new QueryParameter("@pid", data.ProductId)) == 1;
    }
}
