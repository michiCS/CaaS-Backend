using CaaS.Core.Dal.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Dal.Interface;

public interface ICustomerDao
{
    Task<IEnumerable<Customer>> FindAllAsync();
    Task<Customer?> FindByIdAsync(int id);
    Task<bool> InsertAsync(Customer customer);
    Task<Customer?> InsertAndGetAsync(Customer customer);
    Task<Customer?> FindByEmailAndTenantIdAsync(string email, int id);
    Task<IEnumerable<Customer>> FindByTenantIdAsync(int id);
    Task<int> FindRandomCustomerIdByTenantId(int id);
}
