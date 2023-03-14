using CaaS.Core.Dal.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Dal.Interface;

public interface IProductDao
{
    Task<IEnumerable<Product>> FindAllAsync();
    Task<IEnumerable<Product>> FindByTenantIdAsync(int id);
    Task<IEnumerable<Product>> FindAvailableByTenantIdAsync(int id);
    Task<IEnumerable<Product>> FindAvailableByTenantIdPaginationAsync(int id, int limit, int offset);
    Task<int> FindRecordCountForPagination(int id);
    Task<IEnumerable<Product>> FindAvailableBySearchTextAsync(string search, int tenantId);
    Task<Product?> FindByIdAsync(int id);
    Task<bool> InsertAsync(Product product);
    Task<Product?> InsertAndGetAsync(Product product);
    Task<bool> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
    Task<int> FindRandomProductIdByTenantId(int id);
}
