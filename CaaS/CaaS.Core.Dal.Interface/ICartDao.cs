using CaaS.Core.Dal.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Dal.Interface;

public interface ICartDao
{
    Task<IEnumerable<Cart>> FindAllAsync();
    Task<Cart?> FindByIdAsync(int id);
    Task<bool> InsertAsync(Cart cart);
    Task<Cart?> InsertAndGetAsync(Cart cart);
    Task<IEnumerable<Cart>> FindByTenantIdAsync(int id);
    Task<int> FindRandomId();
}
