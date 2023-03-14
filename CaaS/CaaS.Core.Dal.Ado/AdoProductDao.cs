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

public class AdoProductDao : IProductDao
{
    private readonly AdoTemplate template;
    private Product MapRowToProduct(IDataRecord row) =>
        new(
                id: (int)row["id"],
                name: (string)row["name"],
                description: (string)row["description"],
                downloadLink: (string)row["download_link"],
                imageUrl: (string)row["image_url"],
                price: (decimal)row["price"],
                isDeleted: (bool)row["is_deleted"],
                tenantId: (int)row["tenant_id"]
            );

    public AdoProductDao(IConnectionFactory connectionFactory)
    {
        this.template = new AdoTemplate(connectionFactory);
    }

    public async Task<IEnumerable<Product>> FindAllAsync()
    {
        return await template.QueryAsync("select * from product", MapRowToProduct);
    }

    public async Task<Product?> FindByIdAsync(int id)
    {
        return await template.QuerySingleAsync($"select * from product where id=@id", MapRowToProduct, new QueryParameter("@id", id));
    }

    public async Task<IEnumerable<Product>> FindByTenantIdAsync(int id)
    {
        return await template.QueryAsync($"select * from product where tenant_id=@id", MapRowToProduct, new QueryParameter("@id", id));
    }

    public async Task<bool> InsertAsync(Product product)
    {
        return await template.ExecuteAsync(@"insert into product (name, description, download_link, image_url, price, is_deleted, tenant_id) values (@name, @desc, @download, @image, @price, @deleted, @tid)",
            new QueryParameter("@name", product.Name),
            new QueryParameter("@desc", product.Description),
            new QueryParameter("@download", product.DownloadLink),
            new QueryParameter("@image", product.ImageUrl),
            new QueryParameter("@price", product.Price),
            new QueryParameter("@deleted", product.IsDeleted),
            new QueryParameter("@tid", product.TenantId)) == 1;
    }

    public async Task<Product?> InsertAndGetAsync(Product product)
    {
        var id = await template.ExecuteScalarAsync(@"insert into product (name, description, download_link, image_url, price, is_deleted, tenant_id) values (@name, @desc, @download, @image, @price, @deleted, @tid); select last_insert_id()",
                    new QueryParameter("@name", product.Name),
                    new QueryParameter("@desc", product.Description),
                    new QueryParameter("@download", product.DownloadLink),
                    new QueryParameter("@image", product.ImageUrl),
                    new QueryParameter("@price", product.Price),
                    new QueryParameter("@deleted", product.IsDeleted),
                    new QueryParameter("@tid", product.TenantId));

        return id is not null ? await FindByIdAsync(Convert.ToInt32(id)) : null;
    }

    public async Task<int> FindRandomProductIdByTenantId(int id)
    {
        return await template.QuerySingleAsync($"select id from product where tenant_id=@id ORDER BY RAND() LIMIT 1",
            (IDataRecord row) => (int)row["id"], new QueryParameter("@id", id));
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await template.ExecuteAsync(@"update product set is_deleted = true where id = @id",
            new QueryParameter("@id", id)) == 1;
    }

    public async Task<IEnumerable<Product>> FindAvailableByTenantIdAsync(int id)
    {
        return await template.QueryAsync($"select * from product where is_deleted=false and tenant_id=@tid", MapRowToProduct, new QueryParameter("@tid", id));
    }

    public async Task<IEnumerable<Product>> FindAvailableByTenantIdPaginationAsync(int id, int limit, int offset)
    {
        return await template.QueryAsync($"select * from product where is_deleted=false and tenant_id=@tid limit @limit offset @offset", MapRowToProduct,
            new QueryParameter("@tid", id),
            new QueryParameter("@limit", limit),
            new QueryParameter("@offset", offset));
    }

    public async Task<int> FindRecordCountForPagination(int id)
    {
        return Convert.ToInt32((await template.ExecuteScalarAsync($"select count(*) from product where is_deleted=false and tenant_id=@tid", new QueryParameter("@tid", id)))!);
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        return await template.ExecuteAsync(@"update product set name=@name, description=@desc, download_link=@download, image_url=@image, price=@price, is_deleted=@deleted where id=@id",
            new QueryParameter("@id", product.Id),
            new QueryParameter("@name", product.Name),
            new QueryParameter("@desc", product.Description),
            new QueryParameter("@download", product.DownloadLink),
            new QueryParameter("@image", product.ImageUrl),
            new QueryParameter("@price", product.Price),
            new QueryParameter("@deleted", product.IsDeleted)) != 0;
    }

    public async Task<IEnumerable<Product>> FindAvailableBySearchTextAsync(string search, int tenantId)
    {
        return await template.QueryAsync($"select * from product where (name like @search or description like @search) and tenant_id=@tid", MapRowToProduct,
            new QueryParameter("@search", $"%{search}%"),
            new QueryParameter("@tid", tenantId));
    }
}
