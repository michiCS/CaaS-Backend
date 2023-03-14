using CaaS.Core.Dal.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Dal.Interface;

public interface IDiscountActionDataDao
{
    Task<IEnumerable<DiscountActionData>> FindAllAsync();
    Task<DiscountActionData?> FindByIdAsync(int id);
    Task<IEnumerable<DiscountActionData>> FindByTenantIdAsync(int id);
    Task<bool> InsertAsync(DiscountActionData discountAction);
    Task<DiscountActionData?> InsertAndGetAsync(DiscountActionData discountAction);
    Task<bool> UpdateAsync(DiscountActionData discountAction);
    Task<bool> DeleteAsync(int id);
}
