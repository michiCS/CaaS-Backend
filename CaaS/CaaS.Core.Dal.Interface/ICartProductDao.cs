using CaaS.Core.Dal.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Dal.Interface;

public interface ICartProductDao
{
    Task<IEnumerable<CartProduct>> FindAllAsync();
    Task<IEnumerable<CartProduct>> FindByCartIdAsync(int id);
    Task<CartProduct?> FindByIdAsync(int id);
    Task<bool> InsertAsync(CartProduct cartProduct);
    Task<CartProduct?> InsertAndGetAsync(CartProduct cartProduct);
    Task<bool> UpdateQuantityAsync(int id, int quantity);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteByCartIdAsync(int id);
}
