using CaaS.Core.Dal.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Dal.Interface;

public interface IOrderDao
{
    Task<IEnumerable<Order>> FindAllAsync();
    Task<Order?> FindByIdAsync(int id);
    Task<IEnumerable<Order>> FindByTenantIdAsync(int id);
    Task<bool> InsertAsync(Order order);
    Task<Order?> InsertAndGetAsync(Order order);
    Task<bool> UpdateSumDiscounts(int id, decimal sumDiscouts);
    Task<Order?> FindByCartIdAsync(int id);
}
