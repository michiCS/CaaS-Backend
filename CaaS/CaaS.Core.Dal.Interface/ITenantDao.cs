using CaaS.Core.Dal.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Dal.Interface;

public interface ITenantDao
{
    Task<IEnumerable<Tenant>> FindAllAsync();
    Task<Tenant?> FindByIdAsync(int id);
    Task<Tenant?> FindByAppKey(string key);
    Task<bool> InsertAsync(Tenant tenant);
    Task<Tenant?> InsertAndGetAsync(Tenant tenant);
    Task<bool> UpdateAsync(Tenant tenant);
}
